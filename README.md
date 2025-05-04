# 🎬 Video Wallpaper (WPF Edition)

This is a lightweight desktop app that plays a looping video **behind your desktop icons**, turning your static background into a dynamic video wallpaper — just like Lively Wallpaper, but fully custom and lean, built with WPF and Win32 interop.

---

## ✨ Features

- 🖼️ **Live wallpaper** that plays video behind desktop icons
- 🧠 Remembers your last selected video
- 🔧 Tray menu with:
  - Open video
  - Run at startup (modifies Windows registry)
  - Exit
- 🔁 Auto-loop video seamlessly
- 🗂️ Config saved in `%LOCALAPPDATA%\Video Wallpaper\config.txt`

---

## 🛠️ Technologies Used

- **.NET 9.0 (WPF)**
- Native interop via `[LibraryImport]` (P/Invoke source generator)
- `user32.dll` Win32 APIs:
  - `FindWindowExW`, `SetParent`, `SetWindowLongPtrW`, `SendMessageTimeoutW`, `SystemParametersInfoW`
- `MediaElement` for video playback

---

## 🚀 How It Works

1. Finds the hidden `WorkerW` window layer using native API calls
2. Embeds a borderless fullscreen WPF window as a child of `WorkerW`
3. Plays the selected video using `MediaElement` in loop mode
4. Tray icon lets you control the app
5. Can register itself to run at Windows startup

---

## 🧾 How to Build

> Requires Visual Studio 2022+ and .NET 9 SDK

1. Open `VideoWallpaper.sln`
2. Make sure target platform is set to **x64**
3. Build and run the project

---

## 📂 Project Structure

```

VideoWallpaper/
├── Program.cs             # Main logic for video wallpaper and UI
├── VideoWallpaper.csproj # Project file
├── VideoWallpaper.ico    # Application icon (used in tray)
├── VideoWallpaper.sln    # Visual Studio solution
└── obj/                  # (ignored) build artifacts

````

> Add this to your `.gitignore`:
> ```
> bin/
> obj/
> *.user
> ```

---

## 🧪 Tested On

- ✅ Windows 10 (x64)

> Requires Windows to support playback of `.mp4`, `.avi`, etc — relies on system codecs.

---

## 🧙 Credits

Crafted with raw interop magic and WPF.  
No third-party bloat. Just native vibes.

---

## 📄 License

Feel free to use, fork, or modify. No license? No problem. Just don't sell it to aliens.
