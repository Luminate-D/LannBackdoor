using Newtonsoft.Json;

namespace FunModule.Structures.Handlers; 

public class ToggleMonitorHandlerData {
    [JsonProperty("enable")] public bool Enable;
}