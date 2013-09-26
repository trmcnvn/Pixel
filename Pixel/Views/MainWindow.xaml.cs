using System;
using System.Diagnostics;
using System.Windows.Controls;
using Pixel.ViewModels;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace Pixel.Views {
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : IViewFor<MainWindowViewModel> {
    public MainWindow() {
      InitializeComponent();
      ViewModel = new MainWindowViewModel();

      // Hide application in the system tray when minimized
      this.Bind(ViewModel, x => x.IsVisible, x => x.WindowState);
      this.OneWayBind(ViewModel, x => x.IsVisible, x => x.Visibility, () => true, BooleanToVisibilityHint.UseHidden);

      // Binds
      this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
      this.OneWayBind(ViewModel, x => x.IsTopmost, x => x.Topmost);
      this.OneWayBind(ViewModel, x => x.IsVisible, x => x.ShowInTaskbar);
      this.OneWayBind(ViewModel, x => x.ShowCommand, x => x.TaskbarIcon.DoubleClickCommand);

      // Events
      TrayShow.Events().Click.Subscribe(_ => ViewModel.ShowCommand.Execute(null));
      TrayExit.Events().Click.Subscribe(_ => Close());
      MenuExit.Events().Click.Subscribe(_ => Close());
      MenuWebsite.Events().Click.Subscribe(_ => Process.Start("https://github.com/vevix/Pixel"));
      HistoryList.Events().MouseDoubleClick.Subscribe(_ => Process.Start(HistoryList.SelectedValue.ToString()));

      
    }

    #region IViewFor<MainWindowViewModel> Members

    object IViewFor.ViewModel {
      get { return ViewModel; }
      set { ViewModel = value as MainWindowViewModel; }
    }

    public MainWindowViewModel ViewModel { get; set; }

    #endregion
  }
}