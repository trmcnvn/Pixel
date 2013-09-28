using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Pixel.Helpers;
using ReactiveUI;

namespace Pixel.ViewModels {
  public class PreviewWindowViewModel : ReactiveObject {
    public PreviewWindowViewModel(string file) {
      ImageSource = file;
      SaveDlgCommand = new ReactiveCommand();
      CloseCommand = new ReactiveCommand();

      UploadCommand = new ReactiveCommand();
      UploadCommand.Subscribe(async _ => {
        CloseCommand.Execute(null);
        await App.Uploader.Upload(file);
      });

      SaveFileCommand = new ReactiveCommand();
      SaveFileCommand.Subscribe(async x => {
        var saveFile = x as string;
        if (String.IsNullOrEmpty(saveFile)) return;
        await Task.Run(() => {
          using (var image = new Bitmap(ImageSource)) {
            switch (App.Settings.ImageFormat) {
              case ImageFormats.Png:
                image.Save(saveFile, ImageFormat.Png);
                break;
              case ImageFormats.Jpg:
                var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                using (var encParams = new EncoderParameters(1)) {
                  encParams.Param[0] = new EncoderParameter(Encoder.Quality, App.Settings.ImageQuality);
                  if (encoder != null) image.Save(saveFile, encoder, encParams);
                }
                break;
              case ImageFormats.Bmp:
                image.Save(saveFile, ImageFormat.Bmp);
                break;
            }
          }
        });
      });
    }

    public string ImageSource { get; private set; }
    public ReactiveCommand SaveDlgCommand { get; private set; }
    public ReactiveCommand CloseCommand { get; private set; }
    public ReactiveCommand UploadCommand { get; private set; }
    public ReactiveCommand SaveFileCommand { get; private set; }
  }
}