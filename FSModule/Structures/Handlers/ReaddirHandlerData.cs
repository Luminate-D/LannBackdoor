using Newtonsoft.Json;

namespace FSModule.Structures.Handlers;

public class ReaddirHandlerData {
    [JsonProperty("path")] public string Path;
}