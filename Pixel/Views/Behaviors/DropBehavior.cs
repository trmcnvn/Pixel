using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Livet;
using Livet.EventListeners;

namespace Pixel.Views.Behaviors {
  public class DropBehavior : Behavior<FrameworkElement> {
    public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register("DropCommand",
      typeof(ICommand), typeof(DropBehavior), new PropertyMetadata(null));

    private readonly LivetCompositeDisposable _disposable = new LivetCompositeDisposable();

    public ICommand DropCommand {
      get { return (ICommand)GetValue(DropCommandProperty); }
      set { SetValue(DropCommandProperty, value); }
    }

    protected override void OnAttached() {
      base.OnAttached();
      _disposable.Add(new EventListener<DragEventHandler>(handler => AssociatedObject.Drop += handler,
        handler => AssociatedObject.Drop -= handler,
        (s, e) => {
          var command = DropCommand;
          if (command != null && command.CanExecute(e)) {
            command.Execute(e);
          }
        }));
    }

    protected override void OnDetaching() {
      _disposable.Dispose();
      base.OnDetaching();
    }
  }
}