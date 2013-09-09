using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows;
using GlobalHotKey;
using Livet;
using Pixel.Models;
using Pixel.Properties;
using TaskDialogInterop;

namespace Pixel {
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App {
    private static Mutex _appMutex;
    // TODO: Create our own HotKey class
    public static HotKeyManager HotKeyManager = new HotKeyManager();
    public static UploaderManager UploaderManager = new UploaderManager();

    public static string ApplicationName {
      get { return "Pixel"; }
    }

    public static string ApplicationVersion {
      get {
        var ver = Assembly.GetExecutingAssembly().GetName().Version;
        return string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
      }
    }

    public static string RoamingDirectory {
      get {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
      }
    }

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      if (!Directory.Exists(RoamingDirectory)) {
        Directory.CreateDirectory(RoamingDirectory);
      }

      // Livet
      DispatcherHelper.UIDispatcher = Dispatcher;

      // Mutli-code JIT
      ProfileOptimization.SetProfileRoot(RoamingDirectory);
      ProfileOptimization.StartProfile("pixel.profile");

      _appMutex = new Mutex(true, "Pixel-7331B770-095A-4220-924E-8C22B14701E5");
      if (!_appMutex.WaitOne(0, false)) {
        TaskDialog.Show(new TaskDialogOptions {
          Title = ApplicationName,
          MainInstruction = "Application Error",
          Content = string.Format("Sorry, only one instance of {0} may be running at a single time.", ApplicationName),
          MainIcon = VistaTaskDialogIcon.Error,
          CommonButtons = TaskDialogCommonButtons.Close
        });
        Environment.Exit(0);
      }

      UploaderManager.LoadUploaders();
      UploaderManager.Initialize(Settings.Default.ImageUploader);
    }

    protected override void OnExit(ExitEventArgs e) {
      Settings.Default.Save();
      base.OnExit(e);
    }
  }
}