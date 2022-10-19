using Newtonsoft.Json;

namespace FSModule.Structures; 

public class ToggleMouseResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}