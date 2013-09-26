using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pixel.Models {
  public class Uploader {
    private readonly string _clientId;
    private readonly string _uploadPath;

    public Uploader() {
      _clientId = "2243f4383e6516f";
      _uploadPath = "https://api.imgur.com/3/image.xml";
    }

    public event EventHandler<ImgurEventArgs> ImageUploadSuccess;

    protected virtual void OnImageUploadSuccess(ImgurEventArgs e) {
      EventHandler<ImgurEventArgs> handler = ImageUploadSuccess;
      if (handler != null) handler(this, e);
    }

    public event EventHandler<ImgurEventArgs> ImageUploadFailed;

    protected virtual void OnImageUploadFailed(ImgurEventArgs e) {
      EventHandler<ImgurEventArgs> handler = ImageUploadFailed;
      if (handler != null) handler(this, e);
    }

    public async Task Upload(string filePath) {
      using (var webClient = new WebClient()) {
        webClient.Headers.Add("Authorization", string.Format("Client-ID {0}", _clientId));
        var data = await webClient.UploadValuesTaskAsync(new Uri(_uploadPath), new NameValueCollection {
          { "image", Convert.ToBase64String(File.ReadAllBytes(filePath)) }
        });

        using (var ms = new MemoryStream(data)) {
          try {
            var doc = XDocument.Load(ms);
            if (doc.Root != null) {
              var xElement = doc.Root.Element("link");
              if (xElement != null) {
                var link = xElement.Value;
                OnImageUploadSuccess(new ImgurEventArgs {
                  ImageUrl = link
                });
              }
            }
          } catch (Exception ex) {
            OnImageUploadFailed(new ImgurEventArgs {
              Exception = ex
            });
          }
        }
      }
    }
  }

  public class ImgurEventArgs : EventArgs {
    public string ImageUrl { get; set; }
    public Exception Exception { get; set; }
  }
}