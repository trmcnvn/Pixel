using System;
using System.Configuration;
using System.Windows.Input;
using GlobalHotKey;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using Pixel.Extensions;
using Pixel.Properties;
using Pixel.Views.Messaging;
using TaskDialogInterop;

namespace Pixel.ViewModels
{
  public class SettingsWindowViewModel : ViewModel
  {
    private ViewModelCommand _buttonApplyCommand;
    private ViewModelCommand _buttonCloseCommand;
    private ListenerCommand<KeyEventArgs> _screenKeyDownCommand;
    private ListenerCommand<KeyEventArgs> _selectionKeyDownCommand;
    private bool _settingsChanged;

    public SettingsWindowViewModel()
    {
      Settings = Properties.Settings.Default.DeepClone();
      CompositeDisposable.Add(new PropertyChangedEventListener((Settings)Settings, (s, e) => SettingsChanged = true));
    }

    public bool SettingsChanged
    {
      get { return _settingsChanged; }
      set
      {
        if (this.SetIfChanged(ref _settingsChanged, value))
          RaisePropertyChanged(() => SettingsChanged);
      }
    }

    public object Settings { get; private set; }

    #region Commands

    public ViewModelCommand ButtonApplyCommand
    {
      get { return _buttonApplyCommand ?? (_buttonApplyCommand = new ViewModelCommand(ApplySettings)); }
    }

    public ViewModelCommand ButtonCancelCommand
    {
      get
      {
        return _buttonCloseCommand ??
               (_buttonCloseCommand =
                 new ViewModelCommand(() => Messenger.Raise(new WindowActionMessage(WindowAction.Close))));
      }
    }

    // TODO: Can we do this in a single command?
    public ListenerCommand<KeyEventArgs> ScreenKeyUpCommand
    {
      get
      {
        return _screenKeyDownCommand ??
               (_screenKeyDownCommand =
                 new ListenerCommand<KeyEventArgs>(e => ProcessKeyUp("ScreenHotKey", e)));
      }
    }

    public ListenerCommand<KeyEventArgs> SelectionKeyUpCommand
    {
      get
      {
        return _selectionKeyDownCommand ??
               (_selectionKeyDownCommand =
                 new ListenerCommand<KeyEventArgs>(e => ProcessKeyUp("SelectionHotKey", e)));
      }
    }

    #endregion

    private void ProcessKeyUp(string propertyName, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.LeftCtrl:
        case Key.RightCtrl:
        case Key.LeftShift:
        case Key.RightShift:
        case Key.LeftAlt:
        case Key.RightAlt:
        case Key.LWin:
        case Key.RWin:
        case Key.System:
        case Key.Back:
        case Key.Delete:
        case Key.Escape:
          break;
        default:
          var hotKey = new HotKey(e.Key, ModifierKeys.Control | ModifierKeys.Shift);
          Properties.Settings.Default.GetType().GetProperty(propertyName).SetValue(Settings, hotKey);
          break;
      }
    }

    private void ApplySettings()
    {
      // Deal with hotkeys
      try
      {
        App.HotKeyManager.Unregister(Properties.Settings.Default.ScreenHotKey);
        App.HotKeyManager.Unregister(Properties.Settings.Default.SelectionHotKey);

        App.HotKeyManager.Register(((Settings)Settings).ScreenHotKey);
        App.HotKeyManager.Register(((Settings)Settings).SelectionHotKey);
      }
      catch (Exception e)
      {
        Messenger.Raise(new TaskDialogMessage(new TaskDialogOptions
        {
          Title = "Settings",
          MainInstruction = "Application Exception",
          Content = e.Message,
          MainIcon = VistaTaskDialogIcon.Error,
          CommonButtons = TaskDialogCommonButtons.Close
        }));
        return;
      }

      if (!Properties.Settings.Default.ImageUploader.Equals(((Settings)Settings).ImageUploader))
      {
        App.UploaderManager.Initialize(((Settings)Settings).ImageUploader);
      }

      // Apply the settings to Properties.Settings.Default
      foreach (var prop in Properties.Settings.Default.Properties)
      {
        Properties.Settings.Default.GetType()
          .GetProperty(((SettingsProperty)prop).Name)
          .SetValue(Properties.Settings.Default,
            Settings.GetType().GetProperty(((SettingsProperty)prop).Name).GetValue(Settings));
      }

      Properties.Settings.Default.Save();
      ButtonCancelCommand.Execute();
    }
  }
}