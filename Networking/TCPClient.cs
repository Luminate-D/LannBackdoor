using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using LannLogger;
using Networking.Packets;
using Newtonsoft.Json;

namespace Networking;

public class TCPClient {
    public class OnCommandEventArgs : EventArgs {
        public Packet Packet { get; set; }
    }
    
    public int Id;
    
    public event EventHandler OnConnect = delegate {  };
    public event EventHandler<OnCommandEventArgs> OnCommand = delegate {  };

    private readonly TcpClient _client;
    private readonly PacketProcessor _packetProcessor;

    private readonly Serilog.Core.Logger _logger = LoggerFactory.CreateLogger("TcpServer");
    
    public TCPClient() {
        Id = -1;

        _client = new TcpClient();
        _packetProcessor = new PacketProcessor();
    }

    public async Task Connect() {
        await _client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 2022);
        OnConnect(this, EventArgs.Empty);
    }

    public async Task StartHandlingPackets() {
        _logger.Debug("Started handling packets");
        while (true) {
            NetworkStream stream = _client.GetStream();
            
            byte[] buffer = new byte[2048];
            int read = stream.Read(buffer, 0, buffer.Length);

            _packetProcessor.Write(buffer, read);
            Packet? packet = _packetProcessor.TryGetPacket();
            if (packet == null) continue;

            OnCommand(null, new OnCommandEventArgs {
                Packet = packet
            });
        }
    }

    public async Task SendPacket(PacketType type, object data) {
        NetworkStream stream = _client.GetStream();
        string dataString = JsonConvert.SerializeObject(data);

        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
        byte[] typeBytes = BitConverter.GetBytes((int) type);
        byte[] size = BitConverter.GetBytes(dataBytes.Length + typeBytes.Length);
        byte[] resultBytes = size.Concat(typeBytes).Concat(dataBytes).ToArray();
        
        await stream.WriteAsync(resultBytes);
    }
}