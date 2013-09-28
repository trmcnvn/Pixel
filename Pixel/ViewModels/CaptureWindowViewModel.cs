using ReactiveUI;

namespace Pixel.ViewModels {
  public class CaptureWindowViewModel : ReactiveObject {
    public CaptureWindowViewModel() {
      ExitCommand = new ReactiveCommand();
      CaptureCommand = new ReactiveCommand();
    }

    public ReactiveCommand ExitCommand { get; private set; }
    public ReactiveCommand CaptureCommand { get; private set; }
  }
}