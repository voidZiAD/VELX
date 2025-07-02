# <img src="https://github.com/user-attachments/assets/abbe771b-a267-489b-9440-385d8f4b554d" alt="VELX" width="50"/> — Intelligent Game Highlight Sidebar for Windows [Work in Progress]

**VELX** is a modern C#-based sidebar tool for Windows, designed to work like **NVIDIA Highlights**, providing game-aware capture controls, overlay UI, and notification animations — without requiring high-end GPUs.

---

<img src="https://github.com/user-attachments/assets/fc53885b-62d2-4438-baee-7e34a670573a" alt="VELX UI Preview" width="300"/>

---

## ✨ Features

* 💻 **Sidebar UI** with animated sliding
* 🔐 **Hotkey controls**

  * `Alt + X` to toggle sidebar
  * `Esc` to hide sidebar instantly
* 📌 **Always-on-top overlay** with no taskbar clutter
* 🎧 **Microphone control modes**

  * On / Off / Push-To-Talk
* 📁 Panels for:

  * All Clips
  * Games
  * Settings
  * Statistics
* ⚙️ Persistent user settings stored in JSON
* 💬 Custom top-right **toaster-style notifications** for status updates
* 👅 Mouse detection to auto-hide if user clicks outside the overlay
* 🪠 Built using **Windows Forms**, **Guna UI2**, and native Win32 hooks

---

## 📸 Sneak Peek

| Sidebar                             | Notification                                  | Settings                              |
| ----------------------------------- | --------------------------------------------- | ------------------------------------- |
| ![Sidebar](https://github.com/user-attachments/assets/fc53885b-62d2-4438-baee-7e34a670573a) | ![Notification](https://github.com/user-attachments/assets/25b304dd-62bb-454e-89c6-c519bb7e6ef9) | ![Settings](https://github.com/user-attachments/assets/67d10aa2-bae7-47a2-acc4-577b27be7e6b) |

---

## 🛠️ Development / Changes

### 💻 Requirements

* Windows 10/11
* .NET 6.0+ or .NET 8.0
* Visual Studio 2022+
* [Guna UI2 Framework](https://www.nuget.org/packages/Guna.UI2.WinForms/)

---

### ⚖️ Development Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/your-username/VELX.git
   cd VELX
   ```

2. **Install dependencies**

   Ensure Guna UI2 is installed via NuGet:

   ```
   Install-Package Guna.UI2.WinForms
   ```

3. **Build and run**

   Open the `.sln` file in Visual Studio and build the solution. Run `VELX`.

---

## 🧠 How It Works

### 🖦 Sidebar Window

The sidebar uses custom animations to slide in from the left side of the screen. It’s always on top but hidden from the taskbar.

```csharp
ToggleSidebar(); // toggles the form in/out with easing animations
```

### 🎧 Mic Settings

Mic mode rotates between `On`, `Off`, and `Push-To-Talk`:

```csharp
lbMicOptions.Text = "Push-To-Talk";
SettingsManager.SaveSettings(settings);
```

### 🔔 Notifications

Notifications slide in from the upper-right corner:

```csharp
var notif = new VELXNotif("VELX is now in the background", "hidden");
notif.SlideInFromTopRight();
```

---

## 📁 Folder Structure

```
VELX/
├── UI/
│   ├── VELXSidebar.cs         # Main sliding sidebar form
│   ├── VELXNotif.cs           # Custom toast-like notification popup
│   └── Resources/             # Notification icons
├── Properties/
│   └── Resources.resx         # Embedded icons and strings
├── docs/
│   └── images/                # Screenshots for README
├── App.config
└── Program.cs
```

---

## 🧹 Code Highlights

### Slide-In Notification

```csharp
public void SlideInFromTopRight() {
    this.Location = new Point(startX, startY);
    this.Show();
    // Animate to targetX
}
```

### Easing Animations

```csharp
private double EaseOutCubic(double t) => 1 - Math.Pow(1 - t, 3);
```

### System Tray & Context Menu

```csharp
notificationIcon.ContextMenuStrip = hiddenMenu;
notificationIcon.DoubleClick += (s, e) => ToggleSidebar();
```

---

## 🚧 To Do

* [ ] Record game clips
* [ ] Detect kills with OCR
* [ ] Startup boot integration
* [ ] Save video buffers before/after trigger
* [ ] Upload/share clips

---

## 🙌 Acknowledgments

* [Guna UI2 Framework](https://www.nuget.org/packages/Guna.UI2.WinForms/)
* Inspired by **NVIDIA Highlights**
* Built with ❤
* By [voidZiAD](https://github.com/voidZiAD) & [didntpot](https://github.com/didntpot)

---

## 📄 License

MIT License. Feel free to fork and modify.
