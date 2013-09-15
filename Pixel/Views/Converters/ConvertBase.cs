using System;
using System.Globalization;
using System.Windows.Data;

namespace Pixel.Views.Converters
{
  public abstract class ConvertBase
  {
    protected static TDest ConvertSink<TSrc, TDest>(object value, object parameter, Func<TSrc, object, TDest> converter)
    {
      return converter((TSrc)value, parameter);
    }
  }

  public abstract class OneWayConverter<TSrc, TDest> : ConvertBase, IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type destType, object parameter, CultureInfo culture)
    {
      return ConvertSink<TSrc, TDest>(value, parameter, ToTarget);
    }

    public object ConvertBack(object value, Type destType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion

    protected abstract TDest ToTarget(TSrc input, object parameter);
  }

  public abstract class TwoWayConverter<TSrc, TDest> : ConvertBase, IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type destType, object parameter, CultureInfo culture)
    {
      return ConvertSink<TSrc, TDest>(value, parameter, ToTarget);
    }

    public object ConvertBack(object value, Type destType, object parameter, CultureInfo culture)
    {
      return ConvertSink<TDest, TSrc>(value, parameter, ToSource);
    }

    #endregion

    protected abstract TDest ToTarget(TSrc input, object parameter);
    protected abstract TSrc ToSource(TDest input, object parameter);
  }
}