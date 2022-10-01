using Newtonsoft.Json;

namespace FSModule.Structures; 

public class DownloadResult {
    [JsonProperty("rawSuccess")] public bool RawSuccess;
    [JsonProperty("sourceSuccess")] public bool SourceSuccess;
    [JsonProperty("rawError")] public string RawError;
    [JsonProperty("sourceError")] public string SourceError;
}