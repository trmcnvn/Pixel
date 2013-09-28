using System;
using GlobalHotKey;
using ReactiveUI;

namespace Pixel.Views.Converters {
  public class HotKeyToStringConverter : IBindingTypeConverter {
    #region IBindingTypeConverter Members

    public int GetAffinityForObjects(Type lhs, Type rhs) {
      throw new NotImplementedException();
    }

    public bool TryConvert(object @from, Type toType, object conversionHint, out object result) {
      var hk = @from as HotKey;
      result = string.Format("{0}, {1}", hk.Modifiers, hk.Key).Replace(",", " +");
      return true;
    }

    #endregion
  }
}