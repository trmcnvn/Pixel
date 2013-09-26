using System;
using System.Threading.Tasks;

namespace Pixel.Models {
  public interface IUploader {
    string Name { get; }

    void Initialize();
    Task Upload(string filePath);

    event EventHandler<UploaderEventArgs> ImageUploaded;
  }

  public enum UploaderState {
    Success,
    Failed
  }

  public class UploaderEventArgs : EventArgs {
    public UploaderState State { get; set; }
    public Uri ImageUrl { get; set; }
  }
}