using Newtonsoft.Json;

namespace SystemModule.Structures;

public class VerifyHandlerData {
    [JsonProperty("timestamp")] public string Timestamp { get; set; }
    [JsonProperty("signature")] public string Signature { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
}