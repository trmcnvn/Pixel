using System.Collections.Generic;
using Livet;

namespace Pixel.Extensions {
  public static class NotificationObjectEx {
    public static bool SetIfChanged<TObj, TRet>(this TObj This, ref TRet oldValue, TRet newValue)
      where TObj : NotificationObject {
      if (EqualityComparer<TRet>.Default.Equals(oldValue, newValue)) return false;
      oldValue = newValue;
      return true;
    }
  }
}