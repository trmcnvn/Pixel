using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Pixel.Views.Messaging.Behaviors
{
  public class CaptureScreenInteractionMessageAction : InteractionMessageAction<FrameworkElement>
  {
    protected override void InvokeAction(InteractionMessage message)
    {
      var csm = message as CaptureScreenMessage;
      if (csm == null) return;
      if (csm.Width < 10 || csm.Height < 10) return;
      using (var bmpScreenCapture = new Bitmap(csm.Width, csm.Height))
      {
        using (var gfx = Graphics.FromImage(bmpScreenCapture))
        {
          gfx.CopyFromScreen(csm.X, csm.Y, 0, 0, bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);
        }
        var tmpFile = Path.GetTempFileName();
        bmpScreenCapture.Save(tmpFile, ImageFormat.Png);
        csm.Response = tmpFile;
      }
    }
  }
}