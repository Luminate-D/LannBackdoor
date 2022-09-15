using System.Text;
using Newtonsoft.Json;

namespace Networking.Packets; 

public class Packet {
    public readonly int ModuleId;
    public readonly int HandlerId;
    private readonly object _data;
    
    public Packet(List<byte> data) {
        ModuleId = BitConverter.ToInt32(data.Take(4).ToArray());
        HandlerId = BitConverter.ToInt32(data.Skip(4).Take(4).ToArray());

        byte[] dataBytes = data.Skip(8).ToArray();
        string dataString = Encoding.UTF8.GetString(dataBytes);

        _data = JsonConvert.DeserializeObject(dataString) ?? throw new ArgumentException("Cannot parse JSON");
    }

    public T GetData<T>() {
        return (T) _data;
    }
}