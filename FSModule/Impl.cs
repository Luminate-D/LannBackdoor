using FSModule.Structures;
using FSModule.Structures.Handlers;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;

namespace FSModule;

[Module("fs")]
public class FSModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "FileSystem");
    
    [Handler("download")]
    public static async Task DownloadHandler(TCPClient client, DownloadHandlerData data) {
        DownloadResult result = new();
        if (data.Raw != null) {
            byte[] raw = Convert.FromBase64String(data.Raw);

            try {
                await File.WriteAllBytesAsync(data.Path, raw);
                result.RawSuccess = true;
            } catch (Exception error) {
                Logger.Error("Failed to write RAW to {Path}: {Error}",
                    data.Path, error);
                result.RawSuccess = false;
                result.RawError = error.Message;
            }
        }

        if (!string.IsNullOrEmpty(data.SourceURL)) {
            bool valid = Uri.IsWellFormedUriString(data.SourceURL, UriKind.Absolute);
            Uri url = new(data.SourceURL);
            
            if(!valid) {
                Logger.Error("Invalid URL Provided: {URL}", data.SourceURL);
                result.SourceSuccess = false;
                result.SourceError = "Invalid URL Provided";
            }

            try {
                using HttpClient httpClient = new();
                using HttpResponseMessage message = await httpClient.GetAsync(url);
                await using Stream stream = await message.Content.ReadAsStreamAsync();

                FileStream fileStream = File.Create(data.Path);
                await stream.CopyToAsync(fileStream);
                result.SourceSuccess = true;
            } catch (Exception e) {
                result.SourceSuccess = false;
                result.SourceError = e.Message;
            }
        }

        await client.SendPacket(new ClientPacket {
            Type = PacketType.Callback,
            Data = result
        });
    }
}