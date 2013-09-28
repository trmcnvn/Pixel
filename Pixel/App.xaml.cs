using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows;
using GlobalHotKey;
using Microsoft.Win32;
using Pixel.Helpers;
using Pixel.Models;

namespace Pixel {
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App {
    private static Mutex _appMutex;
    public static HotKeyManager HotKeyManager = new HotKeyManager();

    public static string ApplicationName {
      get { return "Pixel"; }
    }

    public static UserSettings Settings { get; private set; }
    public static Uploader Uploader { get; private set; }

    public static string RoamingPath {
      get {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
      }
    }

    public static string SettingsPath {
      get { return Path.Combine(RoamingPath, "Pixel.config"); }
    }

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      _appMutex = new Mutex(true, "Pixel-7331B770-095A-4220-924E-8C22B14701E5");
      if (!_appMutex.WaitOne(0, false)) {
        MessageBox.Show(
          string.Format("Sorry, only one instance of {0} may be running at a single time.", ApplicationName),
          ApplicationName, MessageBoxButton.OK, MessageBoxImage.Information);
        Environment.Exit(0);
      }

      if (!Directory.Exists(RoamingPath))
        Directory.CreateDirectory(RoamingPath);

      ProfileOptimization.SetProfileRoot(RoamingPath);
      ProfileOptimization.StartProfile("Pixel.profile");

      Uploader = new Uploader();
      Settings = UserSettings.Load();

      // Register hotkeys
      try {
        HotKeyManager.Register(Settings.ScreenKey);
        HotKeyManager.Register(Settings.SelectionKey);
      } catch (Exception ex) {
        MessageBox.Show(ex.Message, "Exception");
      }
    }

    protected override void OnExit(ExitEventArgs e) {
      var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
      if (key != null) {
        if (Settings.RunOnStartup) {
          key.SetValue(ApplicationName, Assembly.GetExecutingAssembly().Location);
        } else if (key.GetValueNames().Contains(ApplicationName))
          key.DeleteValue(ApplicationName);
        key.Close();
      }

      foreach (var file in TempFile.Files) {
        File.Delete(file);
      }

      Settings.Save();
      base.OnExit(e);
    }
  }
}