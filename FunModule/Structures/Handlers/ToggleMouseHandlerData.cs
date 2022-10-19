using Newtonsoft.Json;

namespace FunModule.Structures.Handlers; 

public class ToggleMouseHandlerData {
    [JsonProperty("enable")] public bool Enable;
}