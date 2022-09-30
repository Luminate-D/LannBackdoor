using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenModule; 

#pragma warning disable CA1416

public static class Capture {
    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int GetSystemMetrics(int nIndex);

    public static Rectangle GetDimensions() {
        return new Rectangle(0, 0, GetSystemMetrics(0), GetSystemMetrics(1));
    }
    
    public static MemoryStream Screenshot(Rectangle rect) {
        Bitmap bitmap = new(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        Graphics captureGraphics = Graphics.FromImage(bitmap);

        captureGraphics.CopyFromScreen(0, 0, 0, 0
            , new Size(bitmap.Width, bitmap.Height), CopyPixelOperation.SourceCopy);

        MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}

#pragma warning restore CA1416