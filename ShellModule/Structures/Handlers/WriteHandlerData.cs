using Newtonsoft.Json;

namespace ShellModule.Structures.Handlers; 

public class WriteHandlerData {
    [JsonProperty("id")] public int Id;
    [JsonProperty("data")] public string Data;
}