using Newtonsoft.Json;

namespace FSModule.Structures; 

public class ToggleCursorHideResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}