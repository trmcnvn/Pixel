using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using GlobalHotKey;
using Newtonsoft.Json;
using Pixel.Helpers;
using ReactiveUI;

namespace Pixel.Models {
  [DataContract]
  public class UserSettings : ReactiveObject {
    private bool _alwaysOnTop;
    private bool _copyLinks = true;
    private ImageFormats _imageFormat = ImageFormats.Png;
    private int _imageQuality = 100;
    private bool _notifications = true;
    private bool _runOnStartup;
    private HotKey _screenKey = new HotKey(Key.D3, ModifierKeys.Control | ModifierKeys.Shift);
    private HotKey _selectionKey = new HotKey(Key.D4, ModifierKeys.Control | ModifierKeys.Shift);
    private bool _startMinimized;

    [DataMember]
    public bool AlwaysOnTop {
      get { return _alwaysOnTop; }
      set { this.RaiseAndSetIfChanged(ref _alwaysOnTop, value); }
    }

    [DataMember]
    public bool CopyLinks {
      get { return _copyLinks; }
      set { this.RaiseAndSetIfChanged(ref _copyLinks, value); }
    }

    [DataMember]
    public bool RunOnStartup {
      get { return _runOnStartup; }
      set { this.RaiseAndSetIfChanged(ref _runOnStartup, value); }
    }

    [DataMember]
    public bool StartMinimized {
      get { return _startMinimized; }
      set { this.RaiseAndSetIfChanged(ref _startMinimized, value); }
    }

    [DataMember]
    public bool Notifications {
      get { return _notifications; }
      set { this.RaiseAndSetIfChanged(ref _notifications, value); }
    }

    [DataMember]
    public ImageFormats ImageFormat {
      get { return _imageFormat; }
      set { this.RaiseAndSetIfChanged(ref _imageFormat, value); }
    }

    [DataMember]
    public int ImageQuality {
      get { return _imageQuality; }
      set { this.RaiseAndSetIfChanged(ref _imageQuality, value); }
    }

    [DataMember]
    public HotKey ScreenKey {
      get { return _screenKey; }
      set { this.RaiseAndSetIfChanged(ref _screenKey, value); }
    }

    [DataMember]
    public HotKey SelectionKey {
      get { return _selectionKey; }
      set { this.RaiseAndSetIfChanged(ref _selectionKey, value); }
    }

    public static UserSettings Load() {
      try {
        return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(App.SettingsPath, Encoding.UTF8));
      } catch (Exception ex) {
        LogHost.Default.ErrorException("Couldn't load settings, creating them from scratch", ex);
        return new UserSettings();
      }
    }

    public void Save() {
      try {
        File.WriteAllText(App.SettingsPath, JsonConvert.SerializeObject(this), Encoding.UTF8);
      } catch (Exception ex) {
        LogHost.Default.ErrorException("Couldn't save settings", ex);
      }
    }
  }
}