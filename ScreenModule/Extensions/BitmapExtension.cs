using System.Drawing.Imaging;

namespace ScreenModule;

public static class BitmapExtension {
    public static MemoryStream ToMemoryStream(this Bitmap bitmap) {
        MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}