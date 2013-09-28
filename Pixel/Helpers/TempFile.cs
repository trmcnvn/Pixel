using System.Collections.Generic;
using System.IO;

namespace Pixel.Helpers {
  public static class TempFile {
    public static List<string> Files = new List<string>();

    public static string Create() {
      var file = Path.GetTempFileName();
      Files.Add(file);
      return file;
    }
  }
}