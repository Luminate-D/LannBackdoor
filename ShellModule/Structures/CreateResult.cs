using Newtonsoft.Json;

namespace ShellModule.Structures;

public class CreateResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("id")] public int? Id;
    [JsonProperty("error")] public string Error;
}