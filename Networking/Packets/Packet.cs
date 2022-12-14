using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Networking.Packets;

public class Packet {
    public readonly string ModuleName;
    public readonly string HandlerName;
    private readonly JObject _data;

    public Packet(string data) {
        IPacket parsed = JsonConvert.DeserializeObject<IPacket>(data)!;
        ModuleName = parsed.ModuleName;
        HandlerName = parsed.HandlerName;

        _data = parsed.Data;
    }

    public T GetData<T>(Type type) {
        return (T) _data.ToObject(type)!;
    }

    public T GetData<T>() {
        return _data.ToObject<T>()!;
    }
}

public class IPacket {
    [JsonProperty("moduleName")] public string ModuleName { get; set; }
    [JsonProperty("handlerName")] public string HandlerName { get; set; }
    [JsonProperty("data")] public JObject Data { get; set; }
}