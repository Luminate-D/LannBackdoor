using Newtonsoft.Json;

namespace FSModule.Structures;

public class CDRomResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}