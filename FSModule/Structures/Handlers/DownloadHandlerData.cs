using Newtonsoft.Json;

namespace FSModule.Structures.Handlers;

public class DownloadHandlerData {
    [JsonProperty("sourceUrl")] public string? SourceURL;
    [JsonProperty("raw")] public string? Raw;
    [JsonProperty("path")] public string Path;
}