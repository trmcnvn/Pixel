using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Win32;
using Pixel.Messages;
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
      this.OneWayBind(ViewModel, x => x.Title, x => x.TaskbarIcon.ToolTipText);
      this.OneWayBind(ViewModel, x => x.VisiblityCommand, x => x.TaskbarIcon.DoubleClickCommand);
      this.OneWayBind(ViewModel, x => x.ImageHistory, x => x.HistoryList.ItemsSource);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.MenuUpload);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.TrayUpload);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.ButtonBrowse);

      // Commands
      this.WhenAnyObservable(x => x.ViewModel.UploadCommand).Subscribe(_ => {
        var dialog = new OpenFileDialog {
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
          Filter = "Image files (*.jpg, *.gif, *.png, *.bmp, *.tiff, *.pdf)|*.jpg;*.gif;*.png;*.bmp;*.tiff;*.pdf",
          Title = "Upload Images",
          Multiselect = true
        };
        dialog.ShowDialog();
        MessageBus.Current.SendMessage<IEnumerable<string>>(dialog.FileNames);
      });

      // Messages
      MessageBus.Current.Listen<NotificationMessage>()
        .Subscribe(e => TaskbarIcon.ShowBalloonTip(e.Title, e.Text, e.Icon));

      // Events
      TrayShow.Events().Click.Subscribe(_ => ViewModel.VisiblityCommand.Execute(null));
      TrayExit.Events().Click.Subscribe(_ => Close());
      MenuExit.Events().Click.Subscribe(_ => Close());
      MenuWebsite.Events().Click.Subscribe(_ => Process.Start("https://github.com/vevix/Pixel"));
      HistoryList.Events().MouseDoubleClick.Subscribe(_ => Process.Start(HistoryList.SelectedValue.ToString()));
      Dropbox.Events().Drop.Subscribe(e => ViewModel.DropCommand.Execute(e));
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