using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using LannLogger;
using Networking.Packets;
using Networking.Structures;
using Newtonsoft.Json;
using Serilog.Core;

namespace Networking;

public class TCPClient {
    public class OnCommandEventArgs : EventArgs {
        public Packet Packet { get; init; } = null!;
    }

    public int Id;
    public bool IsVerified => Id != -1;

    public event EventHandler OnClose = delegate { };
    public event EventHandler OnConnect = delegate { };
    public event EventHandler<OnCommandEventArgs> OnCommand = delegate { };

    private TcpClient _client;
    private readonly PacketProcessor _packetProcessor;

    private readonly Logger _logger = LoggerFactory.CreateLogger("TcpServer");

    private readonly string _url;
    private readonly int _port;
    private bool _isHandlingPackets;

    public bool IsConnected {
        get {
            try {
                if (_client != null && _client.Client != null && _client.Client.Connected) {
                    if (_client.Client.Poll(0, SelectMode.SelectRead)) {
                        byte[] buff = new byte[1];
                        if (_client.Client.Receive(buff, SocketFlags.Peek) == 0) return false;
                        else return true;
                    }

                    return true;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }
    }

    public TCPClient(string url, int port) {
        _url = url;
        _port = port;
        Id = -1;

        _isHandlingPackets = false;

        _client = new TcpClient();
        _packetProcessor = new PacketProcessor();
    }

    public async Task Connect() {
        await _client.ConnectAsync(IPAddress.Parse(_url), _port);
        OnConnect(this, EventArgs.Empty);
    }

    public void Dispose() {
        _isHandlingPackets = false;
        _client.Close();
        _client.Dispose();

        _client = new TcpClient();
        _packetProcessor.Clear();
        Id = -1;
        
        OnClose(this, EventArgs.Empty);
    }

    public async Task StartHandlingPackets() {
        _logger.Debug("Started handling packets");
        _isHandlingPackets = true;
        while (true) {
            if (!_isHandlingPackets) break;
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

    public async Task SendPacket(ClientPacket data) {
        if (!IsConnected) {
            Dispose();
            return;
        }
        
        NetworkStream stream = _client.GetStream();
        string dataString = JsonConvert.SerializeObject(data);
        byte[] size = BitConverter.GetBytes(dataString.Length);
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);

        try {
            await stream.WriteAsync(size.Concat(dataBytes).ToArray());
        } catch {
            Dispose();
        }
    }
}