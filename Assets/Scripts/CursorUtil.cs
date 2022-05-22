using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pantheon {
  public static class CursorUtil {
    [DllImport("User32.dll", SetLastError = true)]
    [return:MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("User32.dll", SetLastError = true)]
    [return:MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT {
      public int x;
      public int y;
    }

    public static void SetPosition(Vector2 position) {
      if (!SetCursorPos(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y))) {
        Debug.LogError($"Failed to call SetCursorPos(): {Marshal.GetLastWin32Error()}");
      }
    }

    public static Vector2 GetPosition() {
      var point = new POINT();
      if (!GetCursorPos(out point)) {
        Debug.LogError($"Failed to call GetCursorPos(): {Marshal.GetLastWin32Error()}");
        return Vector2.zero;
      }
      return new Vector2(point.x, point.y);
    }
  }
}
