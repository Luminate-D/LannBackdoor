using Newtonsoft.Json;

namespace FSModule.Structures.Handlers;

public class UploadHandlerData {
    [JsonProperty("path")] public string Path;
}