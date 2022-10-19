using System.Drawing.Imaging;
using LannConstants;

namespace ScreenModule;

public class Recorder {
    // public static async Task<string> Record(int frameRate, long duration) {
    //     Rectangle resolution = Screen.GetDimensions();
    //     VideoEncoderSettings settings = new(resolution.Width, resolution.Height, frameRate, VideoCodec.MPEG4);
    //     settings.EncoderPreset = EncoderPreset.Faster;
    //     settings.CRF = 17;
    //     
    //     string timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd_HH:mm:ss");
    //     string outputFileName = Constants.TempFilesPath + "/" + timestamp + ".avi";
    //
    //     MediaOutput output = MediaBuilder.CreateContainer(outputFileName).WithVideo(settings).Create();
    //     DateTime endDate = DateTime.UtcNow.AddMilliseconds(duration);
    //
    //     while (DateTime.UtcNow < endDate) {
    //         Bitmap bmp = GDICapture.CaptureRegion(resolution);
    //         output.Video.AddFrame(FrameToImageData(bmp));
    //         
    //         await Task.Delay(1000 / frameRate);
    //     }
    //     
    //     output.Dispose();
    //
    //     return outputFileName;
    // }
    //
    // private static ImageData FrameToImageData(Bitmap bitmap) {
    //     Rectangle rect = new Rectangle(System.Drawing.Point.Empty, bitmap.Size);
    //     BitmapData bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    //     ImageData bitmapImageData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);
    //     bitmap.UnlockBits(bitLock);
    //     return bitmapImageData;
    // }
}