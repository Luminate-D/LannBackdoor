using Newtonsoft.Json;

namespace ShellModule.Structures; 

public class OutputMessage {
    [JsonProperty("id")] public int Id;
    [JsonProperty("error")] public bool Error;
    [JsonProperty("data")] public string Data;
}