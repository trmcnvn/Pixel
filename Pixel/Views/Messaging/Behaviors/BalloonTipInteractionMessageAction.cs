using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Pixel.Views.Messaging.Behaviors {
  public class BalloonTipInteractionMessageAction : InteractionMessageAction<MainWindow> {
    protected override void InvokeAction(InteractionMessage message) {
      var btm = message as BalloonTipMessage;
      if (btm == null) return;
      AssociatedObject.TaskbarIcon.ShowBalloonTip(btm.Title, btm.Text, btm.Icon);
    }
  }
}