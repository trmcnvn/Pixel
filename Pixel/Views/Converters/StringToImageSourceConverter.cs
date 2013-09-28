using System;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Pixel.Views.Converters {
  public class StringToImageSourceConverter : IBindingTypeConverter {
    #region IBindingTypeConverter Members

    public int GetAffinityForObjects(Type lhs, Type rhs) {
      throw new NotImplementedException();
    }

    public bool TryConvert(object @from, Type toType, object conversionHint, out object result) {
      var file = @from as string;
      var image = new BitmapImage();
      image.BeginInit();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.UriSource = new Uri(file);
      image.EndInit();
      result = image;
      return true;
    }

    #endregion
  }
}