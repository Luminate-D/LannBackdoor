using Networking.Packets;
using Newtonsoft.Json;

namespace Networking.Structures; 

public class ClientPacket {
    [JsonProperty("type")] public PacketType Type { get; set; }
    [JsonProperty("data")] public object?    Data { get; set; }
}