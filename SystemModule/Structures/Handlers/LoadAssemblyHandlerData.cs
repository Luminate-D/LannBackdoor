using Newtonsoft.Json;

namespace SystemModule.Structures;

public class LoadAssemblyHandlerData {
    [JsonProperty("raw")] public string? Raw { get; set; }
    [JsonProperty("path")] public string? Path { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
}