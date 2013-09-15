using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using GlobalHotKey;
using Hardcodet.Wpf.TaskbarNotification;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Pixel.Extensions;
using Pixel.Models;
using Pixel.Properties;
using Pixel.Views;
using Pixel.Views.Messaging;
using TaskDialogInterop;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace Pixel.ViewModels
{
  public class MainWindowViewModel : ViewModel
  {
    private ViewModelCommand _beforeClosingCommand;
    private bool _canClose;
    private ViewModelCommand _captureScreenCommand;
    private ViewModelCommand _captureSelectionCommand;
    private ListenerCommand<DragEventArgs> _dropCommand;
    private ViewModelCommand _exitCommand;
    private bool _isVisibile;
    private ViewModelCommand _settingsCommand;
    private ViewModelCommand _uploadCommand;
    private ViewModelCommand _visibilityCommand;

    public MainWindowViewModel()
    {
      ImageHistory = new ObservableCollection<string>();
      IsVisible = !Settings.Default.StartMinimized;
    }

    #region Commands

    public ViewModelCommand SettingsCommand
    {
      get
      {
        return _settingsCommand ??
               (_settingsCommand =
                 new ViewModelCommand(() =>
                 {
                   var vm = new SettingsWindowViewModel();
                   Messenger.Raise(new TransitionMessage(typeof(SettingsWindow), vm, TransitionMode.Modal,
                     "SettingsWindow"));
                 }));
      }
    }

    public ViewModelCommand UploadCommand
    {
      get { return _uploadCommand ?? (_uploadCommand = new ViewModelCommand(Upload)); }
    }

    public ViewModelCommand ExitCommand
    {
      get
      {
        return _exitCommand ??
               (_exitCommand =
                 new ViewModelCommand(() => Messenger.Raise(new WindowActionMessage(WindowAction.Close))));
      }
    }

    public ViewModelCommand VisibilityCommand
    {
      get
      {
        return _visibilityCommand ??
               (_visibilityCommand =
                 new ViewModelCommand(() => { IsVisible = !IsVisible; }));
      }
    }

    public ViewModelCommand BeforeClosingCommand
    {
      get { return _beforeClosingCommand ?? (_beforeClosingCommand = new ViewModelCommand(RequestClose)); }
    }

    public ViewModelCommand CaptureScreenCommand
    {
      get { return _captureScreenCommand ?? (_captureScreenCommand = new ViewModelCommand(Capture)); }
    }

    public ViewModelCommand CaptureSelectionCommand
    {
      get
      {
        return _captureSelectionCommand ?? (_captureSelectionCommand = new ViewModelCommand(() =>
        {
          var vm = new CaptureWindowViewModel();
          Messenger.Raise(new TransitionMessage(typeof(CaptureWindow), vm, TransitionMode.Normal, "CaptureWindow"));
        }));
      }
    }

    public ListenerCommand<DragEventArgs> DropCommand
    {
      get
      {
        return _dropCommand ??
               (_dropCommand = new ListenerCommand<DragEventArgs>(e =>
               {
                 if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
                 var data = (string[])e.Data.GetData(DataFormats.FileDrop);
                 foreach (var file in data)
                 {
                   App.UploaderManager.ActiveUploader.Upload(file);
                 }
               }));
      }
    }

    #endregion

    public bool IsVisible
    {
      get { return _isVisibile; }
      set
      {
        if (this.SetIfChanged(ref _isVisibile, value))
          RaisePropertyChanged(() => IsVisible);
      }
    }

    public string Title
    {
      get { return string.Format("{0} - {1}", App.ApplicationName, App.ApplicationVersion); }
    }

    public bool CanClose
    {
      get { return _canClose; }
      set
      {
        if (this.SetIfChanged(ref _canClose, value))
          RaisePropertyChanged(() => CanClose);
      }
    }

    public ObservableCollection<string> ImageHistory { get; private set; }

    public void Initialize()
    {
      try
      {
        App.HotKeyManager.Register(Settings.Default.ScreenHotKey);
        App.HotKeyManager.Register(Settings.Default.SelectionHotKey);

        CompositeDisposable.Add(
          new EventListener<EventHandler<KeyPressedEventArgs>>(handler => App.HotKeyManager.KeyPressed += handler,
            handler => App.HotKeyManager.KeyPressed -= handler, OnHotKeyPressed));
        CompositeDisposable.Add(
          new EventListener<EventHandler<UploaderEventArgs>>(
            handler => App.UploaderManager.ActiveUploader.ImageUploaded += handler,
            handler => App.UploaderManager.ActiveUploader.ImageUploaded -= handler, OnImageUploaded));
      }
      catch (Exception e)
      {
        Messenger.Raise(new TaskDialogMessage(new TaskDialogOptions
        {
          Title = Title,
          MainInstruction = "Application Exception",
          Content = "An exception has occurred and the application may not function correctly.",
          ExpandedInfo = e.Message,
          MainIcon = VistaTaskDialogIcon.Error,
          CommonButtons = TaskDialogCommonButtons.Close
        }));
      }
    }

    private void RequestClose()
    {
      if (Settings.Default.ConfirmOnClose)
      {
        var rep = Messenger.GetResponse(new TaskDialogMessage(new TaskDialogOptions
        {
          Title = Title,
          MainInstruction = "Closing Application",
          Content = "Are you sure you want to exit?",
          VerificationText = "Don't show me this message again",
          MainIcon = VistaTaskDialogIcon.Information,
          CommonButtons = TaskDialogCommonButtons.YesNo
        }));

        CanClose = rep.Response != null && rep.Response.Result != TaskDialogSimpleResult.No;
        if (rep.Response != null && rep.Response.VerificationChecked != null)
          Settings.Default.ConfirmOnClose = !rep.Response.VerificationChecked.Value;
      }
      else
      {
        CanClose = true;
      }
    }

    private void OnHotKeyPressed(object sender, KeyPressedEventArgs e)
    {
      var hk = e.HotKey;
      if (hk.Equals(Settings.Default.ScreenHotKey))
      {
        Capture();
      }
      else if (hk.Equals(Settings.Default.SelectionHotKey))
      {
        CaptureSelectionCommand.Execute();
      }
    }


    private void OnImageUploaded(object sender, UploaderEventArgs e)
    {
      if (e.State != UploaderState.Success)
      {
        if (Settings.Default.Popups)
        {
          Messenger.Raise(new BalloonTipMessage(Title, "Image failed to upload.", BalloonIcon.Info));
        }
        return;
      }
      if (Settings.Default.CopyLinks)
      {
        Clipboard.SetText(e.ImageUrl.ToString());
      }
      ImageHistory.Add(e.ImageUrl.ToString());
      if (!Settings.Default.Popups) return;
      var msg = string.Format("Image uploaded: {0}", e.ImageUrl);
      Messenger.Raise(new BalloonTipMessage(Title, msg, BalloonIcon.Info));
    }

    private void Upload()
    {
      var msg = new OpeningFileSelectionMessage
      {
        Title = "Upload Images",
        Filter = "Image files (*.jpg, *.gif, *.png, *.bmp, *.tiff, *.pdf)|*.jpg;*.gif;*.png;*.bmp;*.tiff;*.pdf",
        MultiSelect = true
      };
      var rep = Messenger.GetResponse(msg);
      if (rep.Response == null) return;
      foreach (var file in rep.Response)
      {
        App.UploaderManager.ActiveUploader.Upload(file);
      }
    }

    private void Capture()
    {
      var rep =
        Messenger.GetResponse(new CaptureScreenMessage(Screen.PrimaryScreen.Bounds.Width,
          Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y));
      if (rep.Response == null) return;
      var vm = new PreviewWindowViewModel(rep.Response);
      Messenger.Raise(new TransitionMessage(typeof(PreviewWindow), vm, TransitionMode.Normal, "PreviewWindow"));
    }
  }
}