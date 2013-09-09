using System;
using System.Linq;
using Pixel.Models;

namespace Pixel.Views.Converters {
  public class StringToUploaderConverter : TwoWayConverter<String, IUploader> {
    protected override string ToSource(IUploader input, object parameter) {
      if (input == null) return string.Empty;
      var uploader = App.UploaderManager.Uploaders.FirstOrDefault(u => u.Name == input.Name);
      return uploader == null ? string.Empty : uploader.Name;
    }

    protected override IUploader ToTarget(string input, object parameter) {
      return input == null ? null : App.UploaderManager.Uploaders.FirstOrDefault(u => u.Name == input);
    }
  }
}