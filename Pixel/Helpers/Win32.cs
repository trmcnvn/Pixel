using System;
using System.Runtime.InteropServices;

namespace Pixel.Helpers {
  public class Win32 {
    #region WindowMessages enum

    public enum WindowMessages : uint {
      HOTKEY = 0x0312
    }

    #endregion

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
  }
}