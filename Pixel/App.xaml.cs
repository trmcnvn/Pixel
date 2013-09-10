using System;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows;
using GlobalHotKey;
using Livet;
using Microsoft.Win32;
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
        return string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
      }
    }

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      // Livet
      DispatcherHelper.UIDispatcher = Dispatcher;

      // Mutli-code JIT
      if (ApplicationDeployment.IsNetworkDeployed) {
        ProfileOptimization.SetProfileRoot(ApplicationDeployment.CurrentDeployment.DataDirectory);
        ProfileOptimization.StartProfile("pixel.profile");
      }

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
      // Do the registry work for RunOnStartup
      var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
      if (key != null) {
        if (Settings.Default.RunOnStartup) {
          key.SetValue(ApplicationName, Assembly.GetExecutingAssembly().Location);
        } else if (key.GetValueNames().Contains(ApplicationName))
          key.DeleteValue(ApplicationName);
        key.Close();
      }

      Settings.Default.Save();
      base.OnExit(e);
    }
  }
}