using Newtonsoft.Json;

namespace FunModule.Structures.Handlers; 

public class ToggleCursorHideHandlerData {
    [JsonProperty("enable")] public bool Enable;
}