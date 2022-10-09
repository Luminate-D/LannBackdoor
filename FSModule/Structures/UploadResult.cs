using Newtonsoft.Json;

namespace FSModule.Structures;

public class UploadResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string? Error;
    [JsonProperty("raw")] public string? Raw;
}