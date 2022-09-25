using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Networking.Packets;

public class Packet {
    public readonly  int     ModuleId;
    public readonly  int     HandlerId;
    private readonly JObject _data;

    public Packet(string data) {
        IPacket parsed = JsonConvert.DeserializeObject<IPacket>(data)!;
        ModuleId  = parsed.Module;
        HandlerId = parsed.Handler;

        _data = parsed.Data;
    }

    public T GetData<T>(Type type) {
        return (T)_data.ToObject(type)!;
    }

    public T GetData<T>() {
        return _data.ToObject<T>()!;
    }
}

public class IPacket {
    [JsonProperty("module")]  public int     Module  { get; set; }
    [JsonProperty("handler")] public int     Handler { get; set; }
    [JsonProperty("data")]    public JObject Data    { get; set; }
}