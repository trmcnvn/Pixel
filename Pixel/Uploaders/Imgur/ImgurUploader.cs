using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pixel.Models;

namespace Pixel.Uploaders.Imgur {
  [Export(typeof(IUploader))]
  public class ImgurUploader : IUploader {
    private ImgurClient _client;

    #region IUploader Members

    public string Name {
      get { return "Imgur"; }
    }

    public void Initialize() {
      _client = new ImgurClient("2243f4383e6516f");
    }

    public async Task Upload(string filePath) {
      try {
        var data = await _client.Upload(filePath);
        var doc = XDocument.Load(new MemoryStream(data));
        var link = doc.Root.Element("link").Value;
        OnImageUploaded(new UploaderEventArgs {
          State = UploaderState.Success,
          ImageUrl = new Uri(link)
        });
      } catch (Exception) {
        OnImageUploaded(new UploaderEventArgs {
          State = UploaderState.Failed
        });
      }
    }

    public event EventHandler<UploaderEventArgs> ImageUploaded;

    #endregion

    private void OnImageUploaded(UploaderEventArgs e) {
      var handler = ImageUploaded;
      if (handler != null)
        handler(this, e);
    }
  }
}