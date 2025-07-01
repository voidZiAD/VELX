using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VELX.Properties;
using Timer = System.Windows.Forms.Timer;

namespace VELX.UI
{
    public partial class VELXNotif : Form
    {

        private Timer autoHideTimer;

        public VELXNotif(string label, string type)
        {
            InitializeComponent();

            lbError.Text = label;

            if (type == "error")
                pbIcon.Image = Resources.icons8_error_24;
            else if (type == "saved")
                pbIcon.Image = Resources.icons8_save_24__1_;
            else if (type == "hidden")
                pbIcon.Image = Resources.icons8_hide_24;

        }

        private void StartAutoHideTimer(int milliseconds = 3000)
        {
            if (autoHideTimer != null)
            {
                autoHideTimer.Stop();
                autoHideTimer.Dispose();
            }

            autoHideTimer = new Timer();
            autoHideTimer.Interval = milliseconds;
            autoHideTimer.Tick += (s, e) =>
            {
                autoHideTimer.Stop();
                autoHideTimer.Dispose();
                SlideOutToRight();
            };
            autoHideTimer.Start();
        }


        public void SlideInFromTopRight(int offsetFromTop = 100, int animationSteps = 20, int animationInterval = 10)
        {
            var screen = Screen.FromRectangle(this.Bounds);

            int targetX = screen.Bounds.Right - this.Width; // flush right
            int targetY = screen.Bounds.Top + offsetFromTop;

            int startX = screen.Bounds.Right;
            int startY = targetY;

            this.Location = new Point(startX, startY);
            this.Show();

            int step = 0;

            Timer timer = new Timer();
            timer.Interval = animationInterval;

            timer.Tick += (s, e) =>
            {
                step++;
                double t = (double)step / animationSteps;
                double eased = 1 - Math.Pow(1 - t, 3);
                int currentX = (int)(startX + (targetX - startX) * eased);
                this.Location = new Point(currentX, targetY);

                if (step >= animationSteps)
                {
                    timer.Stop();
                    timer.Dispose();
                    this.Location = new Point(targetX, targetY);

                    StartAutoHideTimer();
                }
            };

            timer.Start();
        }


        public void SlideOutToRight(int animationSteps = 20, int animationInterval = 10)
        {
            var screen = Screen.PrimaryScreen.Bounds;

            int startX = this.Left;
            int targetX = screen.Right;
            int y = this.Top;

            int step = 0;

            Timer timer = new Timer();
            timer.Interval = animationInterval;

            timer.Tick += (s, e) =>
            {
                step++;
                double t = (double)step / animationSteps;
                double eased = Math.Pow(t, 3);
                int currentX = (int)(startX + (targetX - startX) * eased);
                this.Location = new Point(currentX, y);

                if (step >= animationSteps)
                {
                    timer.Stop();
                    timer.Dispose();
                    this.Hide();
                }
            };

            timer.Start();
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
    }
}
