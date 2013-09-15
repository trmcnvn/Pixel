using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Livet.Messaging;

namespace Pixel.Views.Messaging
{
  public class BalloonTipMessage : InteractionMessage
  {
    public BalloonTipMessage(string title, string text, BalloonIcon icon) : this(title, text, icon, null) {}

    public BalloonTipMessage(string title, string text, BalloonIcon icon, string messageKey) : base(messageKey)
    {
      Title = title;
      Text = text;
      Icon = icon;
    }

    public string Title { get; private set; }
    public string Text { get; private set; }
    public BalloonIcon Icon { get; private set; }

    protected override Freezable CreateInstanceCore()
    {
      return new BalloonTipMessage(Title, Text, Icon, MessageKey);
    }
  }
}