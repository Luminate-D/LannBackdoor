using Newtonsoft.Json;

namespace SystemModule.Structures;

public class ModuleLoadResult {
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("error")] public string? Error;
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
}