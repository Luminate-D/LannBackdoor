using Newtonsoft.Json;

namespace FSModule.Structures;

public class BlockInputResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}