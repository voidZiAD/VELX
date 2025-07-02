using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace VELX.UI
{
    public partial class VELXSidebar : Form
    {
        private const int ANIMATION_STEPS = 20; // increase for smoother slide
        private const int ANIMATION_INTERVAL = 5;
        private const int HOTKEY_ID_ALT_X = 0x0002;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_X = 0x58; // ASCII for 'X'
        private bool isVisible = false;
        private int targetX;
        private readonly int fixedWidth = 483;
        private bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

        // Mouse hook related
        private static IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc;

        // Settings object
        private UserSettings settings;

        public VELXSidebar()
        {
            InitializeComponent();

            notificationIcon.Visible = true;
            notificationIcon.ContextMenuStrip = hiddenMenu;
            notificationIcon.DoubleClick += (s, e) => ToggleSidebar(); // Double-click toggles sidebar


            if (!designMode)
            {
                InitializeForm();

                var shadowForm = new Guna.UI2.WinForms.Guna2ShadowForm();
                shadowForm.TargetForm = this;
            }

            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_X, MOD_ALT, VK_X);


            var screen = Screen.FromHandle(this.Handle).Bounds;
            this.SetBounds(screen.Left - fixedWidth, screen.Top, fixedWidth, screen.Height);
            targetX = screen.Left;

            this.Opacity = 0;

            settings = SettingsManager.LoadSettings();

            BeginInvoke(async () =>
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
                this.Opacity = 1;
                await Task.Delay(100);
                ToggleSidebar();

                lbMicOptions.Text = settings.MicOption;
                UpdateMicOptionLocation();
            });

            _proc = HookCallback;

            gradientTextLabel2.MouseEnter += (s, e) => btnRecord.FillColor = btnRecord.HoverState.FillColor;
            gradientTextLabel2.MouseLeave += (s, e) => btnRecord.FillColor = Color.Transparent;

            gradientTextLabel3.MouseEnter += (s, e) => btnGames.FillColor = btnGames.HoverState.FillColor;
            gradientTextLabel3.MouseLeave += (s, e) => btnGames.FillColor = Color.Transparent;

            gradientTextLabel4.MouseEnter += (s, e) => btnTextDetectionSaving.FillColor = btnTextDetectionSaving.HoverState.FillColor;
            gradientTextLabel4.MouseLeave += (s, e) => btnTextDetectionSaving.FillColor = Color.Transparent;
            pnIntro.BringToFront();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED, for double buffering
                return cp;
            }
        }


        private void SlideInPanel(Panel panel, Control? optionalException)
        {
            panel.Dock = DockStyle.None;
            panel.Size = pnMain.Size;

            // Set correct size and position of the loading panel before parenting
            pnGeneralLoading.Size = new Size(483, 723);
            pnGeneralLoading.Location = new Point(0, 69);
            pnGeneralLoading.Visible = true;

            // Temporarily parent the loading panel to the panel being slid
            this.Controls.Remove(pnGeneralLoading);
            panel.Controls.Add(pnGeneralLoading);
            pnGeneralLoading.BringToFront();

            // Optionally hide other controls during animation
            foreach (Control ctl in panel.Controls)
            {
                if (ctl != pnGeneralLoading && ctl != optionalException)
                    ctl.Visible = false;
            }

            int panelWidth = panel.Width;
            int y = panel.Top;

            int startX = -panelWidth;
            int endX = 0;

            int step = 0;
            int steps = 20;
            int interval = 5;

            panel.Location = new Point(startX, y);
            panel.Visible = true;
            panel.BringToFront();

            Timer animationTimer = new Timer();
            animationTimer.Interval = interval;

            animationTimer.Tick += (s, e) =>
            {
                double t = (double)step / steps;
                double eased = EaseOutCubic(t);
                int x = (int)(startX + (endX - startX) * eased);

                panel.Location = new Point(x, y);
                step++;

                if (step > steps)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();

                    panel.Location = new Point(endX, y);
                    panel.Dock = DockStyle.Fill;

                    async void FinishSlide()
                    {
                        panel.Controls.Remove(pnGeneralLoading);
                        this.Controls.Add(pnGeneralLoading);

                        await Task.Delay(200); // 👈 Adjust delay here in milliseconds
                        pnGeneralLoading.Visible = false;

                        foreach (Control ctl in panel.Controls)
                            ctl.Visible = true;
                    }

                    FinishSlide();
                }
            };

            animationTimer.Start();
        }



        private void HidePanelInstantly(Panel panel)
        {
            panel.SendToBack();
            panel.Visible = false;
            panel.Location = new Point(-panel.Width, panel.Top); // Reset position off-screen
        }

        // Optional: easing function (same as before)
        private double EaseOutCubic(double t) => 1 - Math.Pow(1 - t, 3);


        private void InitializeForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;
        }

        // Added optional parameter to indicate instant show (no animation)
        public void ToggleSidebar(bool showInstant = false, Action? onShown = null)

        {
            if (!IsHandleCreated) return;

            bool wasVisible = isVisible;
            isVisible = !isVisible;

            if (isVisible)
            {
                RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NONE, VK_ESCAPE);
                InstallMouseHook();

                pnIntro.Visible = true;      // ✅ Force show the intro
                pnIntro.BringToFront();      // ✅ Bring to front before sliding
            }
            else
            {
                var notif = new VELXNotif(@"VELX is now hiding in the
background", "hidden");
                notif.SlideInFromTopRight();
                UnregisterHotKey(this.Handle, HOTKEY_ID);
                UninstallMouseHook();
            }

            if (showInstant)
            {
                int x = isVisible ? targetX : targetX - this.Width;
                this.SetDesktopBounds(x, this.Top, this.Width, this.Height);

                if (isVisible)
                {
                    pnIntro.Visible = false;
                    pnMain?.BringToFront();
                }

                return;
            }

            BeginInvoke(async () => await AnimateSidebarAsync(wasVisible, onShown));
        }



        private async Task AnimateSidebarAsync(bool wasVisible, Action? onShown)
        {
            int start = wasVisible ? this.Left : this.Left;
            int end = isVisible ? targetX : targetX - this.Width;

            for (int i = 0; i <= ANIMATION_STEPS; i++)
            {
                double t = (double)i / ANIMATION_STEPS;
                double eased = wasVisible ? EaseInCubic(t) : EaseOutCubic(t);
                int x = (int)(start + (end - start) * eased);

                this.SetDesktopBounds(x, this.Top, this.Width, this.Height);
                await Task.Delay(ANIMATION_INTERVAL);
            }

            this.Left = end;

            if (isVisible)
            {
                await Task.Delay(100);
                pnIntro.Visible = false;
                pnMain?.BringToFront();

                onShown?.Invoke(); // ✅ Call the action (e.g., to show settings)
            }
        }



        private double EaseInCubic(double t) => Math.Pow(t, 3);

        // -------------------------
        // Mouse hook setup

        private void InstallMouseHook()
        {
            if (_hookID == IntPtr.Zero)
            {
                _hookID = SetHook(_proc);
            }
        }

        private void UninstallMouseHook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                    Point pt = new Point(hookStruct.pt.x, hookStruct.pt.y);

                    if (isVisible && !this.Bounds.Contains(pt))
                    {
                        BeginInvoke(() =>
                        {
                            ToggleSidebar();
                        });
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // -------------------------
        // Hotkey (ESC) handling

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 0x0001;
        private const uint MOD_NONE = 0x0000;
        private const uint VK_ESCAPE = 0x1B;
        private const int WM_HOTKEY = 0x0312;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == HOTKEY_ID)
                {
                    ToggleSidebar(); // ESC
                    var notif = new VELXNotif("VELX is now in the background", "hidden");
                    notif.SlideInFromTopRight();
                    return;
                }
                else if ((int)m.WParam == HOTKEY_ID_ALT_X)
                {
                    pnIntro.BringToFront();
                    ToggleSidebar(); // ALT + X
                    return;
                }
            }

            base.WndProc(ref m);
        }


        // -------------------------
        // Window positioning

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const int SWP_NOACTIVATE = 0x0010;
        const int SWP_SHOWWINDOW = 0x0040;
        private bool allowExit = false;

        private bool isExiting = false; // <-- NEW flag

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isExiting)
            {
                base.OnFormClosing(e);
                return;
            }

            if (!allowExit)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }

            e.Cancel = true;
            this.Hide();

            isExiting = true;
            PerformDelayedExit();
        }

        private async void PerformDelayedExit()
        {
            notificationIcon.Visible = false;
            notificationIcon.Dispose();
            var notif = new VELXNotif(@"VELX is closing and will no
longer record clips. Goodbye!", "wave");
            notif.SlideInFromTopRight();

            await Task.Delay(4000); // Let user see the notif

            // Cleanup
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_X);
            UninstallMouseHook();

            allowExit = true;

            this.Close();
        }




        // -------------------------
        // Mic Option label and buttons

        private void UpdateMicOptionLocation()
        {
            if (lbMicOptions.Text == "On")
                lbMicOptions.Location = new Point(328, 620);
            else if (lbMicOptions.Text == "Off")
                lbMicOptions.Location = new Point(328, 620);
            else if (lbMicOptions.Text == "Push-To-Talk")
            {
                lbMicOptions.Location = new Point(282, 620);
                lbMicrophoneDesc.Text = @"Press the key P to start
or stop recording your mic.";
            }
        }

        private void btnLeftMicrophone_Click(object sender, EventArgs e)
        {
            if (lbMicOptions.Text == "On")
            {
                lbMicOptions.Text = "Off";
                lbMicrophoneDesc.Text = @"CTRL+Shift+M | On/Off";
            }
            else if (lbMicOptions.Text == "Off")
            {
                lbMicOptions.Text = "Push-To-Talk";
                lbMicrophoneDesc.Text = @"Press the key P to start
or stop recording your mic.";
            }
            else if (lbMicOptions.Text == "Push-To-Talk")
            {
                lbMicOptions.Text = "On";
                lbMicrophoneDesc.Text = @"CTRL+Shift+M | On/Off";
            }

            UpdateMicOptionLocation();
            settings.MicOption = lbMicOptions.Text;
            SettingsManager.SaveSettings(settings);
        }

        private void btnRightMicrophone_Click(object sender, EventArgs e)
        {
            if (lbMicOptions.Text == "On")
            {
                lbMicOptions.Text = "Off";
                lbMicrophoneDesc.Text = @"CTRL+Shift+M | On/Off";
            }
            else if (lbMicOptions.Text == "Push-To-Talk")
            {
                lbMicOptions.Text = "On";
                lbMicrophoneDesc.Text = @"CTRL+Shift+M | On/Off";
            }
            else if (lbMicOptions.Text == "Off")
            {
                lbMicOptions.Text = "Push-To-Talk";
                lbMicrophoneDesc.Text = @"Press the key P to start
or stop recording your mic.";
            }

            UpdateMicOptionLocation();
            settings.MicOption = lbMicOptions.Text;
            SettingsManager.SaveSettings(settings);
        }

        private async void CreateError(string error)
        {
            lbError.Text = error;
            pnGeneralError.BringToFront();
            pnGeneralError.Visible = true;

            await Task.Delay(1500);

            pnGeneralError.Hide();
        }


        private void btnRecord_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ToggleSidebar();
        }

        private void btnAllClips_Click(object sender, EventArgs e)
        {
            SlideInPanel(pnAllClips, guna2GradientPanel1);
        }

        private void btnACBack_Click(object sender, EventArgs e)
        {
            HidePanelInstantly(pnAllClips);
        }

        private void btnGames_Click(object sender, EventArgs e)
        {
            SlideInPanel(pnGames, guna2GradientPanel2);
        }

        private void btnGBack_Click(object sender, EventArgs e)
        {
            HidePanelInstantly(pnGames);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SlideInPanel(pnSettings, guna2GradientPanel3);
        }

        private void btnSTBack_Click(object sender, EventArgs e)
        {
            HidePanelInstantly(pnSettings);
        }
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            SlideInPanel(pnStatistics, guna2GradientPanel4);
        }

        private void btnSBack_Click(object sender, EventArgs e)
        {
            HidePanelInstantly(pnStatistics);
        }

        private void btnRightClips_Click(object sender, EventArgs e)
        {
            CreateError("There are no current clips.");
        }

        private void btnLeftClips_Click(object sender, EventArgs e)
        {
            CreateError("There are no current clips.");
        }

        private void showAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSidebar();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowExit = true;
            Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSidebar(false, () =>
            {
                btnSettings.PerformClick();
            });
        }

        private void AllClipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSidebar(false, () =>
            {
                btnAllClips.PerformClick();
            });
        }

        private void StatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSidebar(false, () =>
            {
                btnStatistics.PerformClick();
            });
        }
    }

    // -------------------------
    // Settings classes & manager

    public class UserSettings
    {
        public string MicOption { get; set; } = "On";
    }

    public static class SettingsManager
    {
        private static readonly string appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VELX", "settings");

        private static readonly string settingsFilePath = Path.Combine(appDataFolder, "options.json");

        public static UserSettings LoadSettings()
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                if (!File.Exists(settingsFilePath))
                {
                    var defaultSettings = new UserSettings();
                    SaveSettings(defaultSettings);
                    return defaultSettings;
                }

                string json = File.ReadAllText(settingsFilePath);
                return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
            catch
            {
                return new UserSettings();
            }
        }

        public static void SaveSettings(UserSettings settings)
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, json);
            }
            catch
            {

            }
        }
    }
}