using System;
using System.Reactive.Linq;
using GlobalHotKey;
using ReactiveUI;

namespace Pixel.ViewModels {
  public class MainWindowViewModel : ReactiveObject {
    private bool _isVisible;

    public MainWindowViewModel() {
      IsVisible = !App.Settings.StartMinimized;

      ShowCommand = new ReactiveCommand();
      ShowCommand.Subscribe(_ => IsVisible = !IsVisible);

      // HotKeys
      Observable.FromEventPattern<KeyPressedEventArgs>(handler => App.HotKeyManager.KeyPressed += handler,
        handler => App.HotKeyManager.KeyPressed -= handler).Select(x => x.EventArgs).Subscribe(e => {
          var hk = e.HotKey;
          if (hk.Equals(App.Settings.ScreenKey)) {
            // 
          } else if (hk.Equals(App.Settings.SelectionKey)) {
            //
          }
        });

      App.Settings.ObservableForProperty(x => x.AlwaysOnTop)
        .Subscribe(_ => this.RaisePropertyChanged("IsTopmost"));
    }

    public string Title {
      get { return App.ApplicationName; }
    }

    public bool IsTopmost {
      get { return App.Settings.AlwaysOnTop; }
    }

    public bool IsVisible {
      get { return _isVisible; }
      set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
    }

    public ReactiveCommand ShowCommand { get; private set; }
  }
}