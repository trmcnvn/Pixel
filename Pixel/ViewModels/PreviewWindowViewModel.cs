using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Livet;
using Livet.Commands;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Pixel.Extensions;
using Pixel.Helpers;
using Pixel.Properties;

namespace Pixel.ViewModels {
  public class PreviewWindowViewModel : ViewModel {
    private ViewModelCommand _exitCommand;
    private string _imagePath;
    private ViewModelCommand _saveCommand;
    private ViewModelCommand _uploadCommand;

    public PreviewWindowViewModel(string filePath) {
      _imagePath = filePath;
    }

    public string ImagePath {
      get { return _imagePath; }
      set {
        if (this.SetIfChanged(ref _imagePath, value))
          RaisePropertyChanged(() => ImagePath);
      }
    }

    #region Commands

    public ViewModelCommand ExitCommand {
      get {
        return _exitCommand ??
               (_exitCommand = new ViewModelCommand(() => Messenger.Raise(new WindowActionMessage(WindowAction.Close))));
      }
    }

    public ViewModelCommand SaveCommand {
      get {
        return _saveCommand ??
               (_saveCommand = new ViewModelCommand(SaveImage));
      }
    }

    public ViewModelCommand UploadCommand {
      get {
        return _uploadCommand ??
               (_uploadCommand = new ViewModelCommand(() => {
                 var tmpFile = Path.GetTempFileName();
                 Save(tmpFile);
                 App.UploaderManager.ActiveUploader.Upload(tmpFile);
                 ExitCommand.Execute();
               }));
      }
    }

    #endregion

    private void SaveImage() {
      var rep = Messenger.GetResponse(new SavingFileSelectionMessage {
        Title = "Save Image",
        Filter = "All Files (*.*)|*.*",
        FileName =
          string.Format("{0}.{1}", DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"),
            Settings.Default.ImageFormat.ToString().ToLower()),
        OverwritePrompt = true
      });
      if (rep.Response == null) return;
      var file = rep.Response[0];
      Save(file);
    }

    private void Save(string file) {
      using (var image = new Bitmap(ImagePath)) {
        switch (Settings.Default.ImageFormat) {
          case ImageFormats.Png:
            image.Save(file, ImageFormat.Png);
            break;
          case ImageFormats.Jpg:
            var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
            using (var encParams = new EncoderParameters(1)) {
              encParams.Param[0] = new EncoderParameter(Encoder.Quality, Settings.Default.ImageQuality);
              image.Save(file, encoder, encParams);
            }
            break;
          case ImageFormats.Bmp:
            image.Save(file, ImageFormat.Bmp);
            break;
        }
      }
    }
  }
}