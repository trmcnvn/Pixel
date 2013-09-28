using System.Windows;

namespace Pixel.Messages {
  public class MessageBoxMessage {
    public MessageBoxMessage(string title, string text, MessageBoxImage icon, MessageBoxButton buttons) {
      Title = title;
      Text = text;
      Icon = icon;
      Buttons = buttons;
    }

    public string Title { get; private set; }
    public string Text { get; private set; }
    public MessageBoxImage Icon { get; private set; }
    public MessageBoxButton Buttons { get; private set; }
  }
}