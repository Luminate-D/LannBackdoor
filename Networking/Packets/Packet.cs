using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Networking.Packets; 

public class Packet {
    public readonly int ModuleId;
    public readonly int HandlerId;
    public readonly bool IsDataRaw;
    private readonly object _data;
    
    public Packet(List<byte> data) {
        ModuleId = BitConverter.ToInt32(data.Take(4).ToArray());
        HandlerId = BitConverter.ToInt32(data.Skip(4).Take(4).ToArray());
        IsDataRaw = data.Skip(8).First() == 1;
        
        byte[] dataBytes = data.Skip(9).ToArray();

        if (IsDataRaw) {
            _data = dataBytes;
            return;
        }

        string dataString = Encoding.UTF8.GetString(dataBytes);
        _data = JsonConvert.DeserializeObject(dataString) ?? throw new ArgumentException("Cannot parse JSON");
    }

    public T GetData<T>(Type type) {
        JObject jObject = (JObject)_data;
        return (T) jObject.ToObject(type)!;
    }
    
    public T GetData<T>() {
        return (T) _data;
    }
}