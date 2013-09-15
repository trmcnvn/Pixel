using System.Windows;
using System.Windows.Input;
using Livet.Behaviors;

namespace Pixel.Extensions
{
  public class WindowCloseCancelBehaviorEx : WindowCloseCancelBehavior
  {
    // Using a DependencyProperty as the backing store. This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BeforeClosingCommandProperty =
      DependencyProperty.Register("BeforeClosingCommand", typeof(ICommand), typeof(WindowCloseCancelBehaviorEx),
        new PropertyMetadata(null));

    public static readonly DependencyProperty BeforeClosingMethodTargetProperty =
      DependencyProperty.Register("BeforeClosingMethodTarget", typeof(object), typeof(WindowCloseCancelBehaviorEx),
        new PropertyMetadata(null));

    public static readonly DependencyProperty BeforeClosingMethodNameProperty =
      DependencyProperty.Register("BeforeClosingMethodName", typeof(string), typeof(WindowCloseCancelBehaviorEx),
        new PropertyMetadata(null));

    private readonly MethodBinder _beforeClosingMethod = new MethodBinder();

    public ICommand BeforeClosingCommand
    {
      get { return (ICommand)GetValue(BeforeClosingCommandProperty); }
      set { SetValue(BeforeClosingCommandProperty, value); }
    }

    public object BeforeClosingMethodTarget
    {
      get { return GetValue(BeforeClosingMethodTargetProperty); }
      set { SetValue(BeforeClosingMethodTargetProperty, value); }
    }

    public string BeforeClosingMethodName
    {
      get { return (string)GetValue(BeforeClosingMethodNameProperty); }
      set { SetValue(BeforeClosingMethodNameProperty, value); }
    }

    protected override void OnAttached()
    {
      AssociatedObject.Closing += (sender, e) =>
      {
        if (BeforeClosingCommand != null && BeforeClosingCommand.CanExecute(null))
        {
          BeforeClosingCommand.Execute(null);
        }
        if (BeforeClosingMethodTarget != null && BeforeClosingMethodName != null)
        {
          _beforeClosingMethod.Invoke(BeforeClosingMethodTarget, BeforeClosingMethodName);
        }
      };

      base.OnAttached();
    }
  }
}