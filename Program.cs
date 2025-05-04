using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Win32;
using System.IO;

using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

partial class Program
{
  [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
  private static partial IntPtr FindWindowExW(IntPtr parent, IntPtr childAfter, string className, string windowTitle);

  [LibraryImport("user32.dll", SetLastError = true)]
  private static partial IntPtr SendMessageTimeoutW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

  [LibraryImport("user32.dll")]
  private static partial IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

  [LibraryImport("user32.dll")]
  private static partial int SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

  [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static partial bool SystemParametersInfoW(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

  [STAThread]
  static void Main()
  {
    var progman = FindWindowExW(0, 0, "Progman", null);
    SendMessageTimeoutW(progman, 0x052C, 0xD, 0x1, 0, 1000, 0);

    var handle = IntPtr.Zero;
    while (true)
    {
      handle = FindWindowExW(0, handle, "WorkerW", null);
      var defView = FindWindowExW(handle, 0, "SHELLDLL_DefView", null);
      if (defView != 0)
        break;
    }
    var workerW = FindWindowExW(0, handle, "WorkerW", null);

    var application = new Application();

    var location = Environment.ProcessPath;
    var appName = "Video Wallpaper";

    var configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
    if (!Directory.Exists(configDir))
      Directory.CreateDirectory(configDir);

    var config = Path.Combine(configDir, "config.txt");
    var lastVideo = File.Exists(config)
      ? File.ReadAllText(config)
      : null;

    application.Startup += (sender0, e0) =>
    {
      var media = new MediaElement
      {
        LoadedBehavior = MediaState.Manual,
        Volume = 0
      };

      if (lastVideo != null)
        media.Source = new Uri(lastVideo);

      media.MediaEnded += (sender, e) =>
      {
        media.Position = TimeSpan.FromMilliseconds(1);
        media.Play();
      };

      var window = new Window
      {
        Title = appName,
        WindowStartupLocation = WindowStartupLocation.Manual,
        Left = 0,
        Top = 0,
        Width = SystemParameters.PrimaryScreenWidth,
        Height = SystemParameters.PrimaryScreenHeight,
        ShowInTaskbar = false,
        Background = Brushes.Black,
        Content = new Grid { Children = { media } }
      };

      window.Loaded += (sender, e) =>
      {
        media.Play();
      };

      window.Show();

      var hWnd = new WindowInteropHelper(window).Handle;
      SetWindowLongPtrW(hWnd, -16, 0x50000000);
      SetParent(hWnd, workerW);

      var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

      var notify = new NotifyIcon
      {
        Visible = true,
        Text = appName,
        Icon = Icon.ExtractAssociatedIcon(location),
        ContextMenuStrip = new ContextMenuStrip
        {
          RenderMode = ToolStripRenderMode.System,
          Items =
          {
            new ToolStripMenuItem("Open Video...", null, (sender, e) =>
            {
              var dialog = new OpenFileDialog
              {
                Filter = "Video Files|*.mp4;*.wmv;*.avi;*.mpeg;*.mpg|All Files|*.*"
              };
              if (dialog.ShowDialog() == DialogResult.OK)
              {
                media.Source = new Uri(dialog.FileName);
                File.WriteAllText(config, dialog.FileName);
              }
            }),
            new ToolStripMenuItem("Run at Startup", null, (sender, e) =>
            {
              var item = sender as ToolStripMenuItem;
              item.Checked = !item.Checked;
              if (item.Checked)
                key.SetValue(appName, location);
              else
                key.DeleteValue(appName, false);
            }) {
              Checked = key.GetValue(appName) != null
            },
            new ToolStripMenuItem("Exit", null, (sender, e) =>
            {
              window.Close();
            }),
          }
        }
      };

      application.Exit += (sender, e) =>
      {
        media.Close();
        notify.Dispose();
        SystemParametersInfoW(0x14, 0, null, 0x3);
      };
    };

    application.Run();
  }
}
