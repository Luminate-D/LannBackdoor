using Newtonsoft.Json;

namespace FSModule.Structures; 

public class ToggleMonitorResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}