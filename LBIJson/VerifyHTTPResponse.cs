using Newtonsoft.Json;

namespace LBIJson {
    public class VerifyHTTPResponse {
        [JsonProperty("timestamp")] public long TimeStamp;
        [JsonProperty("signature")] public string Signature;
    }
}