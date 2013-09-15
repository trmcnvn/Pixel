using System.Windows;
using Livet.Messaging;
using TaskDialogInterop;

namespace Pixel.Views.Messaging
{
  public class TaskDialogMessage : ResponsiveInteractionMessage<TaskDialogResult>
  {
    public TaskDialogMessage(TaskDialogOptions options, string messageKey) : base(messageKey)
    {
      Options = options;
    }

    public TaskDialogMessage(TaskDialogOptions options)
    {
      Options = options;
    }

    public TaskDialogOptions Options { get; private set; }

    protected override Freezable CreateInstanceCore()
    {
      return new TaskDialogMessage(Options, MessageKey);
    }
  }
}