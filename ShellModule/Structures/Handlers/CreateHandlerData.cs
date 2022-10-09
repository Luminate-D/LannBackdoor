using Newtonsoft.Json;

namespace ShellModule.Structures.Handlers;

public class CreateHandlerData {
    [JsonProperty("fileName")] public string FileName;
}