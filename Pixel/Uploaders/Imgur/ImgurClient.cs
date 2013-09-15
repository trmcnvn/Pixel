using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Pixel.Uploaders.Imgur
{
  public class ImgurClient
  {
    private readonly string _clientId;
    private readonly Uri _uploadUri = new Uri("https://api.imgur.com/3/image.xml");

    public ImgurClient(string clientId)
    {
      _clientId = clientId;
    }

    public async Task<byte[]> Upload(string filePath)
    {
      using (var webClient = new WebClient())
      {
        webClient.Headers.Add("Authorization", string.Format("Client-ID {0}", _clientId));
        return await webClient.UploadValuesTaskAsync(_uploadUri, new NameValueCollection
        {
          { "image", Convert.ToBase64String(File.ReadAllBytes(filePath)) }
        });
      }
    }
  }
}