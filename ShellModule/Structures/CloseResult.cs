using Newtonsoft.Json;

namespace ShellModule.Structures;

public class CloseResult {
    [JsonProperty("id")] public int Id;
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
}