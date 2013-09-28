using System;
using System.Windows;
using ReactiveUI;

namespace Pixel.Views.Converters {
  /// <summary>
  ///   Translates true to WindowState.Normal and false to WindowState.Minimized
  /// </summary>
  public class BooleanToWindowStateConverter : IBindingTypeConverter {
    #region IBindingTypeConverter Members

    public int GetAffinityForObjects(Type lhs, Type rhs) {
      throw new NotImplementedException();
    }

    public bool TryConvert(object @from, Type toType, object conversionHint, out object result) {
      if (toType == typeof(WindowState)) {
        var boolValue = !(@from is bool) || (bool)@from;
        result = boolValue ? WindowState.Normal : WindowState.Minimized;
        return true;
      }
      var windowState = @from is WindowState ? (WindowState)@from : WindowState.Normal;
      result = windowState != WindowState.Minimized;
      return true;
    }

    #endregion
  }
}