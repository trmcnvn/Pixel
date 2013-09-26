using System.Windows;
using Livet.Messaging;

namespace Pixel.Views.Messaging {
  public class CaptureScreenMessage : ResponsiveInteractionMessage<string> {
    public CaptureScreenMessage(int width, int height, int x, int y) : this(width, height, x, y, null) {}

    public CaptureScreenMessage(int width, int height, int x, int y, string messageKey)
      : base(messageKey) {
      Width = width;
      Height = height;
      X = x;
      Y = y;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    protected override Freezable CreateInstanceCore() {
      return new CaptureScreenMessage(Width, Height, X, Y, MessageKey);
    }
  }
}