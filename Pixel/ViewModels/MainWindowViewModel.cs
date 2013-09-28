using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using GlobalHotKey;
using Hardcodet.Wpf.TaskbarNotification;
using Pixel.Messages;
using Pixel.Models;
using ReactiveUI;

namespace Pixel.ViewModels {
  public class MainWindowViewModel : ReactiveObject {
    private bool _isVisible;

    public MainWindowViewModel() {
      IsVisible = !App.Settings.StartMinimized;
      ImageHistory = new ReactiveList<string>();

      VisiblityCommand = new ReactiveCommand();
      DropCommand = new ReactiveCommand();
      UploadCommand = new ReactiveCommand();
      ScreenCommand = new ReactiveCommand();

      VisiblityCommand.Subscribe(_ => IsVisible = !IsVisible);

      DropCommand.Subscribe(async ev => {
        var e = ev as DragEventArgs;
        if (e == null) return;
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        var data = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
        if (data == null) return;
        foreach (var file in data) {
          await App.Uploader.Upload(file);
        }
      });

      OpenCommand = new ReactiveCommand();
      OpenCommand.Subscribe(async files => {
        foreach (var file in (IEnumerable<string>)files) {
          await App.Uploader.Upload(file);
        }
      });

      Observable.FromEventPattern<KeyPressedEventArgs>(handler => App.HotKeyManager.KeyPressed += handler,
        handler => App.HotKeyManager.KeyPressed -= handler).Select(x => x.EventArgs).Subscribe(e => {
          var hk = e.HotKey;
          if (hk.Equals(App.Settings.ScreenKey)) {
            ScreenCommand.Execute(null);
          } else if (hk.Equals(App.Settings.SelectionKey)) {
            //
          }
        });

      Observable.FromEventPattern<UploaderEventArgs>(handler => App.Uploader.ImageUploadSuccess += handler,
        handler => App.Uploader.ImageUploadSuccess -= handler).Select(x => x.EventArgs)
        .Subscribe(e => {
          if (App.Settings.CopyLinks) Clipboard.SetText(e.ImageUrl);
          ImageHistory.Add(e.ImageUrl);
          if (!App.Settings.Notifications) return;
          var msg = string.Format("Image Uploaded: {0}", e.ImageUrl);
          MessageBus.Current.SendMessage(new NotificationMessage(Title, msg, BalloonIcon.Info));
        });

      Observable.FromEventPattern<UploaderEventArgs>(handler => App.Uploader.ImageUploadFailed += handler,
        handler => App.Uploader.ImageUploadFailed -= handler).Select(x => x.EventArgs)
        .Subscribe(e => {
          if (!App.Settings.Notifications) return;
          LogHost.Default.ErrorException("Failed to upload image", e.Exception);
          var msg = string.Format("Image Failed: {0}", e.Exception.Message);
          MessageBus.Current.SendMessage(new NotificationMessage(Title, msg,
            BalloonIcon.Error));
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

    public ReactiveList<string> ImageHistory { get; private set; }
    public ReactiveCommand VisiblityCommand { get; private set; }
    public ReactiveCommand DropCommand { get; private set; }
    public ReactiveCommand UploadCommand { get; private set; }
    public ReactiveCommand ScreenCommand { get; private set; }
    public ReactiveCommand OpenCommand { get; private set; }
  }
}