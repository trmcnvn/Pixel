using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Livet;
using Livet.EventListeners;

namespace Pixel.Views.Behaviors {
  public class KeyUpBehavior : Behavior<FrameworkElement> {
    public static readonly DependencyProperty KeyUpCommandProperty = DependencyProperty.Register("KeyUpCommand",
      typeof(ICommand), typeof(KeyUpBehavior), new PropertyMetadata(null));

    private readonly LivetCompositeDisposable _disposable = new LivetCompositeDisposable();

    public ICommand KeyUpCommand {
      get { return (ICommand)GetValue(KeyUpCommandProperty); }
      set { SetValue(KeyUpCommandProperty, value); }
    }

    protected override void OnAttached() {
      base.OnAttached();
      _disposable.Add(new EventListener<KeyEventHandler>(
        handler => AssociatedObject.KeyUp += handler,
        handler => AssociatedObject.KeyUp -= handler, (s, e) => {
          var command = KeyUpCommand;
          if (command != null && command.CanExecute(e)) {
            command.Execute(e);
          }
        }));
    }

    protected override void OnDetaching() {
      base.OnDetaching();
      _disposable.Dispose();
    }
  }
}