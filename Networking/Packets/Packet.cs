using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Networking.Packets; 

public class Packet {
    public readonly int ModuleId;
    public readonly int HandlerId;
    private readonly object _data;
    
    public Packet(string data) {
        IPacket parsed = JsonConvert.DeserializeObject<IPacket>(data)!;
        ModuleId = parsed.Module;
        HandlerId = parsed.Handler;

        _data = parsed.Data;
    }

    public T GetData<T>(Type type) {
        JObject jObject = (JObject)_data;
        return (T) jObject.ToObject(type)!;
    }
    
    public T GetData<T>() {
        return (T) _data;
    }
}


public class IPacket {
    [JsonProperty("module")] public int Module { get; set; }
    [JsonProperty("handler")] public int Handler { get; set; }
    [JsonProperty("data")] public object Data { get; set; }
}