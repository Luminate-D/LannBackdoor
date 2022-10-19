using System.Runtime.InteropServices;

namespace ScreenModule;

public class Screen {
    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int GetSystemMetrics(int nIndex);

    public static Rectangle GetDimensions() {
        return new Rectangle(0, 0, GetSystemMetrics(0), GetSystemMetrics(1));
    }
}