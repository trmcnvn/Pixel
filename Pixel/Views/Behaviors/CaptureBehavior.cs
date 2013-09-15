using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Livet;
using Livet.EventListeners;

namespace Pixel.Views.Behaviors
{
  public class CaptureBehavior : Behavior<CaptureWindow>
  {
    private static readonly DependencyProperty CaptureCommandProperty = DependencyProperty.Register("CaptureCommand",
      typeof(ICommand), typeof(CaptureBehavior), new PropertyMetadata(null));

    private readonly LivetCompositeDisposable _disposable = new LivetCompositeDisposable();

    public ICommand CaptureCommand
    {
      get { return (ICommand)GetValue(CaptureCommandProperty); }
      set { SetValue(CaptureCommandProperty, value); }
    }

    protected override void OnAttached()
    {
      base.OnAttached();
      _disposable.Add(new EventListener<MouseButtonEventHandler>(
        handler => AssociatedObject.MouseLeftButtonDown += handler,
        handler => AssociatedObject.MouseLeftButtonDown -= handler, MouseLeftButtonDown));
      _disposable.Add(new EventListener<MouseEventHandler>(
        handler => AssociatedObject.MouseMove += handler,
        handler => AssociatedObject.MouseMove -= handler, MouseMove));
      _disposable.Add(new EventListener<MouseButtonEventHandler>(
        handler => AssociatedObject.MouseLeftButtonUp += handler,
        handler => AssociatedObject.MouseLeftButtonUp -= handler, MouseLeftButtonUp));
    }

    protected override void OnDetaching()
    {
      _disposable.Dispose();
      base.OnDetaching();
    }

    private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var point = e.GetPosition(AssociatedObject);
      Canvas.SetLeft(AssociatedObject.Target, point.X);
      Canvas.SetTop(AssociatedObject.Target, point.Y);
    }

    private void MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton != MouseButtonState.Pressed) return;
      var point = e.GetPosition(AssociatedObject);
      var width = point.X - Canvas.GetLeft(AssociatedObject.Target);
      var height = point.Y - Canvas.GetTop(AssociatedObject.Target);
      if (!(width > 0) || !(height > 0)) return;
      AssociatedObject.Target.Width = width;
      AssociatedObject.Target.Height = height;
    }

    private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      AssociatedObject.Target.Visibility = Visibility.Collapsed;
      var command = CaptureCommand;
      if (command != null && command.CanExecute(AssociatedObject.Target))
      {
        command.Execute(AssociatedObject.Target);
      }
    }
  }
}