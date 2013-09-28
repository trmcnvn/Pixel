using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using Pixel.Helpers;
using Pixel.Messages;
using Pixel.ViewModels;
using Pixel.Views.Converters;
using ReactiveUI;
using ReactiveUI.Xaml;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Pixel.Views {
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : IViewFor<MainWindowViewModel> {
    public MainWindow() {
      InitializeComponent();
      ViewModel = new MainWindowViewModel();

      this.Bind(ViewModel, x => x.IsVisible, x => x.WindowState, null, new BooleanToWindowStateConverter(),
        new BooleanToWindowStateConverter());
      this.OneWayBind(ViewModel, x => x.IsVisible, x => x.Visibility, () => true, BooleanToVisibilityHint.UseHidden);

      this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
      this.OneWayBind(ViewModel, x => x.IsTopmost, x => x.Topmost);
      this.OneWayBind(ViewModel, x => x.IsVisible, x => x.ShowInTaskbar);
      this.OneWayBind(ViewModel, x => x.Title, x => x.TaskbarIcon.ToolTipText);
      this.OneWayBind(ViewModel, x => x.VisiblityCommand, x => x.TaskbarIcon.DoubleClickCommand);
      this.OneWayBind(ViewModel, x => x.ImageHistory, x => x.HistoryList.ItemsSource);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.MenuUpload);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.TrayUpload);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.ButtonBrowse);
      this.BindCommand(ViewModel, x => x.ScreenCommand, x => x.ButtonScreen);
      this.BindCommand(ViewModel, x => x.SelectionCommand, x => x.ButtonSelection);

      this.WhenAnyObservable(x => x.ViewModel.UploadCommand).Subscribe(_ => {
        var dialog = new OpenFileDialog {
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
          Filter = "Image files (*.jpg, *.gif, *.png, *.bmp, *.tiff, *.pdf)|*.jpg;*.gif;*.png;*.bmp;*.tiff;*.pdf",
          Title = "Upload Images",
          Multiselect = true
        };
        dialog.ShowDialog();
        ViewModel.OpenCommand.Execute(dialog.FileNames);
      });

      this.WhenAnyObservable(x => x.ViewModel.ScreenCommand)
        .Subscribe(async _ => {
          var file = await CaptureScreen.Capture(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
            Screen.PrimaryScreen.Bounds.Width,
            Screen.PrimaryScreen.Bounds.Height);

          var previewWindow = new PreviewWindow(file) { Owner = this };
          previewWindow.Show();
        });

      this.WhenAnyObservable(x => x.ViewModel.SelectionCommand).Subscribe(_ => {
        var captureWindow = new CaptureWindow { Owner = this };
        captureWindow.Show();
      });

      TrayShow.Events().Click.Subscribe(_ => ViewModel.VisiblityCommand.Execute(null));
      TrayExit.Events().Click.Subscribe(_ => Close());
      MenuExit.Events().Click.Subscribe(_ => Close());
      MenuWebsite.Events().Click.Subscribe(_ => Process.Start("https://github.com/vevix/Pixel"));
      HistoryList.Events().MouseDoubleClick.Subscribe(_ => Process.Start(HistoryList.SelectedValue.ToString()));
      Dropbox.Events().Drop.Subscribe(e => ViewModel.DropCommand.Execute(e));

      MessageBus.Current.Listen<NotificationMessage>()
        .Subscribe(e => TaskbarIcon.ShowBalloonTip(e.Title, e.Text, e.Icon));
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