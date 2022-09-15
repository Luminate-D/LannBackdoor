using System.Net.Sockets;
using LannLogger;
using Networking;
using Networking.Packets;
using Newtonsoft.Json;
using Serilog.Core;
using Constants = LannConstants.Constants;

namespace LannBackdoor;

public static class LannBackdoor {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("LannBackdoor");
    private static TCPClient _tcpClient = null!;

    public static async Task Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        if(Constants.DEBUG) Logger.Debug("Running {Mode} mode", "DEBUG");
        Logger.Information("Ohayou, ばか!");
        Logger.Information("Config:\n  - Debug: {Debug}\n  - Url: {URL}\n  - Port: {Port}",
            true,
            "127.0.0.1",
            2022);
        _tcpClient = new TCPClient();

        _tcpClient.OnConnect += async (_, _) => {
            Logger.Information("Connected!");
            await _tcpClient.SendPacket(PacketType.System, new {
                ProductName = "Elbow Grease",
                Enabled = true,
                StockCount = 9000
            });
        };

        await Connect();
    }

    private static async Task Connect() {
        try {
            await _tcpClient.Connect();
            await _tcpClient.StartHandlingPackets();
        } catch (SocketException) {
            Logger.Error("Failed to connect! Reconnecting in 5000 ms");
            await Task.Delay(5000);
            await Connect();
        }
    }
}