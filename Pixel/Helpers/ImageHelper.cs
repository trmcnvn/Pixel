using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Pixel.Helpers {
  public enum ImageFormats {
    Png,
    Jpg,
    Bmp
  }

  public static class CaptureScreen {
    public static async Task<string> Capture(int x, int y, int width, int height) {
      if (width < 10 || height < 10) {
        return string.Empty;
      }
      return await Task.Run(() => {
        using (var bmp = new Bitmap(width, height)) {
          using (var gfx = Graphics.FromImage(bmp)) {
            gfx.CopyFromScreen(x, y, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
          }
          var file = TempFile.Create();
          bmp.Save(file, ImageFormat.Png);
          return file;
        }
      });
    }
  }
}