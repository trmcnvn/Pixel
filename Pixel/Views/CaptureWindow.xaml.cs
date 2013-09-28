using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using Pixel.Helpers;
using Pixel.ViewModels;
using ReactiveUI;
using MouseButtonState = System.Windows.Input.MouseButtonState;
using Visibility = System.Windows.Visibility;

namespace Pixel.Views {
  /// <summary>
  ///   Interaction logic for CaptureWindow.xaml
  /// </summary>
  public partial class CaptureWindow : IViewFor<CaptureWindowViewModel> {
    public CaptureWindow() {
      InitializeComponent();
      ViewModel = new CaptureWindowViewModel();

      this.BindCommand(ViewModel, x => x.ExitCommand, x => x.EscapeKey);
      this.WhenAnyObservable(x => x.ViewModel.ExitCommand).Subscribe(_ => Close());
      this.WhenAnyObservable(x => x.ViewModel.CaptureCommand).Subscribe(async r => {
        Close();
        var rect = r as Rectangle;
        var file =
          await
            CaptureScreen.Capture((int)Canvas.GetLeft(rect), (int)Canvas.GetTop(rect), (int)rect.Width, (int)rect.Height);

        var previewWindow = new PreviewWindow(file);
        previewWindow.Show();
      });

      EventsMixin.Events(this).MouseLeftButtonDown.Subscribe(e => {
        var point = e.GetPosition(this);
        Canvas.SetLeft(Target, point.X);
        Canvas.SetTop(Target, point.Y);
      });

      EventsMixin.Events(this).MouseMove.Where(x => x.LeftButton == MouseButtonState.Pressed).Subscribe(e => {
        var point = e.GetPosition(this);
        var width = point.X - Canvas.GetLeft(Target);
        var height = point.Y - Canvas.GetTop(Target);
        if (!(width > 0) || !(height > 0)) {
          return;
        }
        Target.Width = width;
        Target.Height = height;
      });

      EventsMixin.Events(this).MouseLeftButtonUp.Subscribe(e => {
        Target.Visibility = Visibility.Collapsed;
        ViewModel.CaptureCommand.Execute(Target);
      });

      EventsMixin.Events(this).Unloaded.Subscribe(_ => MessageBus.Current.SendMessage<object>(null, "CaptureWindow"));
    }

    #region IViewFor<CaptureWindowViewModel> Members

    object IViewFor.ViewModel {
      get { return ViewModel; }
      set { ViewModel = value as CaptureWindowViewModel; }
    }

    public CaptureWindowViewModel ViewModel { get; set; }

    #endregion
  }
}