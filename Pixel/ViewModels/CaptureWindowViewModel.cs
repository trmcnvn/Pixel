using System.Windows.Controls;
using System.Windows.Shapes;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Pixel.Views;
using Pixel.Views.Messaging;

namespace Pixel.ViewModels {
  public class CaptureWindowViewModel : ViewModel {
    private ListenerCommand<Rectangle> _captureCommand;
    private ViewModelCommand _exitCommand;

    public ViewModelCommand ExitCommand {
      get {
        return _exitCommand ??
               (_exitCommand = new ViewModelCommand(() => Messenger.Raise(new WindowActionMessage(WindowAction.Close))));
      }
    }

    public ListenerCommand<Rectangle> CaptureCommand {
      get {
        return _captureCommand ?? (_captureCommand = new ListenerCommand<Rectangle>(r => {
          Messenger.Raise(new WindowActionMessage(WindowAction.Close));
          var rep =
            Messenger.GetResponse(new CaptureScreenMessage((int)r.Width, (int)r.Height, (int)Canvas.GetLeft(r),
              (int)Canvas.GetTop(r)));
          if (rep.Response == null) return;
          var vm = new PreviewWindowViewModel(rep.Response);
          Messenger.Raise(new TransitionMessage(typeof(PreviewWindow), vm, TransitionMode.Normal, "PreviewWindow"));
        }));
      }
    }
  }
}