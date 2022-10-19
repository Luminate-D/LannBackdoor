using Newtonsoft.Json;

namespace SystemModule.Structures;

public class AssemblyLoadResult {
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("fullName")] public string? FullName { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("error")] public string? Error;
}