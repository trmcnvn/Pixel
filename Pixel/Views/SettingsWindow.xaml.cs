using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GlobalHotKey;
using Pixel.Messages;
using Pixel.ViewModels;
using Pixel.Views.Converters;
using ReactiveUI;

namespace Pixel.Views {
  /// <summary>
  ///   Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : IViewFor<SettingsWindowViewModel> {
    public SettingsWindow() {
      InitializeComponent();
      ViewModel = new SettingsWindowViewModel();

      this.Bind(ViewModel, x => x.Settings.AlwaysOnTop, x => x.AlwaysOnTop.IsChecked);
      this.Bind(ViewModel, x => x.Settings.CopyLinks, x => x.CopyLinks.IsChecked);
      this.Bind(ViewModel, x => x.Settings.RunOnStartup, x => x.RunOnStartup.IsChecked);
      this.Bind(ViewModel, x => x.Settings.StartMinimized, x => x.StartMinimized.IsChecked);
      this.Bind(ViewModel, x => x.Settings.Notifications, x => x.Notifications.IsChecked);
      this.Bind(ViewModel, x => x.Settings.ImageFormat, x => x.ImageFormat.SelectedItem);
      this.Bind(ViewModel, x => x.Settings.ImageQuality, x => x.QualitySlider.Value);
      this.OneWayBind(ViewModel, x => x.Settings.ScreenKey, x => x.ScreenKey.Text, null, null,
        new HotKeyToStringConverter());
      this.OneWayBind(ViewModel, x => x.Settings.SelectionKey, x => x.SelectionKey.Text, null, null,
        new HotKeyToStringConverter());
      this.BindCommand(ViewModel, x => x.CloseCommand, x => x.ButtonClose);

      this.WhenAnyObservable(x => x.ViewModel.CloseCommand).Subscribe(_ => Close());
      this.WhenAnyObservable(x => x.ViewModel.Settings.Changed).Subscribe(e => TextStatus.Text = "Settings Saved");

      MessageBus.Current.Listen<MessageBoxMessage>().Subscribe(x => MessageBox.Show(x.Text, x.Title, x.Buttons, x.Icon));

      ScreenKey.Events().PreviewKeyUp.Subscribe(e => {
        var hk = ProcessKey(e);
        ViewModel.KeyCommand.Execute(Tuple.Create("ScreenKey", hk));
      });

      SelectionKey.Events().PreviewKeyUp.Subscribe(e => {
        var hk = ProcessKey(e);
        ViewModel.KeyCommand.Execute(Tuple.Create("SelectionKey", hk));
      });
    }

    #region IViewFor<SettingsWindowViewModel> Members

    object IViewFor.ViewModel {
      get { return ViewModel; }
      set { ViewModel = value as SettingsWindowViewModel; }
    }

    public SettingsWindowViewModel ViewModel { get; set; }

    #endregion

    private static HotKey ProcessKey(KeyEventArgs e) {
      switch (e.Key) {
        case Key.LeftCtrl:
        case Key.RightCtrl:
        case Key.LeftShift:
        case Key.RightShift:
        case Key.LeftAlt:
        case Key.RightAlt:
        case Key.LWin:
        case Key.RWin:
        case Key.System:
        case Key.Back:
        case Key.Delete:
        case Key.Escape:
          break;
        default:
          return new HotKey(e.Key, ModifierKeys.Control | ModifierKeys.Shift);
      }
      return null;
    }
  }
}