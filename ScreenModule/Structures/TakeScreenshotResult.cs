using Newtonsoft.Json;

namespace ScreenModule.Structures;

public class TakeScreenshotResult {
    [JsonProperty("success")] public bool Success;
    [JsonProperty("error")] public string Error;
    [JsonProperty("data")] public byte[] Data;
}