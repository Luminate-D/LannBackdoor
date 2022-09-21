using System.Text;

namespace Networking.Packets;

enum PacketProcessorState {
    Awaiting, Processing
}

public class PacketProcessor {
    private List<byte> _buffer;
    private PacketProcessorState _state;
    private int _size;
    
    public PacketProcessor() {
        _buffer = new List<byte>();
        _state = PacketProcessorState.Awaiting;
        _size = -1;
    }

    public void Clear() {
        _buffer = new List<byte>();
        _size = -1;
        _state = PacketProcessorState.Awaiting;
    }

    public void Write(byte[] data, int count) {
        for (int i = 0; i < count; i++) {
            _buffer.Add(data[i]);
        }
    }

    public Packet? TryGetPacket() {
        if (_state == PacketProcessorState.Awaiting) {
            if (_buffer.Count < 4) return null;

            _size = BitConverter.ToInt32(_buffer.Take(4).ToArray());
            _buffer = _buffer.Skip(4).ToList();
            
            _state = PacketProcessorState.Processing;
        }

        if (_state == PacketProcessorState.Processing) {
            if (_buffer.Count < _size) return null;

            byte[] readData = _buffer.Take(_size).ToArray();
            _buffer = _buffer.Skip(_size).ToList();

            _state = PacketProcessorState.Awaiting;
            _size = -1;

            return new Packet(Encoding.UTF8.GetString(readData));
        }

        return null;
    }
}