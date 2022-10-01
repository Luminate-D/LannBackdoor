using System.Runtime.InteropServices;

namespace LannBackdoor; 

public static class PInvoke {
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void HideWindow() {
        ShowWindow(GetConsoleWindow(), 0);
    }
}