using System;
using System.Windows;
using GlobalHotKey;
using Pixel.Messages;
using Pixel.Models;
using ReactiveUI;

namespace Pixel.ViewModels {
  public class SettingsWindowViewModel : ReactiveObject {
    public SettingsWindowViewModel() {
      CloseCommand = new ReactiveCommand();

      KeyCommand = new ReactiveCommand();
      KeyCommand.Subscribe(x => {
        var tuple = x as Tuple<string, HotKey>;
        if (tuple.Item2 == null) return;

        App.HotKeyManager.Unregister(Settings.ScreenKey);
        App.HotKeyManager.Unregister(Settings.SelectionKey);

        var tmpScreenKey = Settings.ScreenKey;
        var tmpSelectionKey = Settings.SelectionKey;
        switch (tuple.Item1) {
          case "ScreenKey":
            tmpScreenKey = tuple.Item2;
            break;
          case "SelectionKey":
            tmpSelectionKey = tuple.Item2;
            break;
        }

        App.HotKeyManager.Register(tmpScreenKey);
        App.HotKeyManager.Register(tmpSelectionKey);

        // Only apply the change if there hasn't been an error registering them
        Settings.GetType().GetProperty(tuple.Item1).SetValue(Settings, tuple.Item2);
      });
      KeyCommand.ThrownExceptions.Subscribe(
        ex => {
          // We have to re-register the hotkeys here so
          // that both are registered with the system
          App.HotKeyManager.Unregister(Settings.ScreenKey);
          App.HotKeyManager.Unregister(Settings.SelectionKey);
          App.HotKeyManager.Register(Settings.ScreenKey);
          App.HotKeyManager.Register(Settings.SelectionKey);

          MessageBus.Current.SendMessage(new MessageBoxMessage(App.ApplicationName, ex.Message, MessageBoxImage.Error,
            MessageBoxButton.OK));
        });

      App.Settings.Changed.Subscribe(_ => this.RaisePropertyChanged("Settings"));
    }

    public ReactiveCommand CloseCommand { get; private set; }
    public ReactiveCommand KeyCommand { get; private set; }

    public UserSettings Settings {
      get { return App.Settings; }
    }
  }
}