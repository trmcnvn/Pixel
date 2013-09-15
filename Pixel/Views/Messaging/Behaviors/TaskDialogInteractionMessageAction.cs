using System.Windows;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using TaskDialogInterop;

namespace Pixel.Views.Messaging.Behaviors
{
  public class TaskDialogInteractionMessageAction : InteractionMessageAction<FrameworkElement>
  {
    protected override void InvokeAction(InteractionMessage message)
    {
      var tdm = message as TaskDialogMessage;
      if (tdm == null) return;
      var options = tdm.Options;
      var result = TaskDialog.Show(options);
      tdm.Response = result;
    }
  }
}