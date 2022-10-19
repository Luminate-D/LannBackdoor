using System.Runtime.InteropServices;

namespace ScreenModule;

public static class GDICapture {
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc,
        int nXSrc, int nYSrc, int dwRop);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

    private const int SRCCOPY = 0x00CC0020;
    private const int CAPTUREBLT = 0x40000000;

    public static Bitmap CaptureRegion(Rectangle region) {
        IntPtr desktophWnd = GetDesktopWindow();
        IntPtr desktopDc = GetWindowDC(desktophWnd);
        IntPtr memoryDc = CreateCompatibleDC(desktopDc);
        IntPtr bitmap = CreateCompatibleBitmap(desktopDc, region.Width, region.Height);
        IntPtr oldBitmap = SelectObject(memoryDc, bitmap);

        bool success = BitBlt(memoryDc,
            0,
            0,
            region.Width,
            region.Height,
            desktopDc,
            region.Left,
            region.Top,
            SRCCOPY | CAPTUREBLT);

        if (!success) return null;
        Bitmap result = Image.FromHbitmap(bitmap);

        SelectObject(memoryDc, oldBitmap);
        DeleteObject(bitmap);
        DeleteDC(memoryDc);
        ReleaseDC(desktophWnd, desktopDc);

        return result;
    }
}