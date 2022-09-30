using Newtonsoft.Json;

namespace ScreenModule.Structures; 

public class TakeScreenshotResult {
    [JsonProperty("data")] public byte[] Data;
}