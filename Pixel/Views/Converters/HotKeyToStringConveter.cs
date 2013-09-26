using GlobalHotKey;

namespace Pixel.Views.Converters {
  public class HotKeyToStringConverter : OneWayConverter<HotKey, string> {
    protected override string ToTarget(HotKey input, object parameter) {
      return input == null ? string.Empty : string.Format("{0}, {1}", input.Modifiers, input.Key).Replace(",", " +");
    }
  }
}