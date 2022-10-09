using Newtonsoft.Json;

namespace FSModule.Structures;

public class ReaddirResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string? Error;
    [JsonProperty("files")] public string[]? Files;
    [JsonProperty("directories")] public string[]? Directories;
}