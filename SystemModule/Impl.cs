using System.Reflection;
using System.Text;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Serilog.Core;
using SystemModule.Structures;

namespace SystemModule;

[ModulesApi.Module("system")]
public class SystemModuleImpl {
    private static bool _verified;
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "System");
    
    [Handler("ping")]
    public static async Task PongHandler(TCPClient client, object data) {
        await client.SendPacket(PacketType.Pong, new { });
    }
    
    [Handler("verify")]
    public static async Task VerifyHandler(TCPClient client, VerifyHandlerData data) {
        if (_verified) throw new Exception("Verify packet received twice or more.");
        // TODO: Verify signature
        _verified = true;
        await client.SendPacket(PacketType.Verified, new { });
    }

    [Handler("loadAssembly")]
    public static async Task LoadAssemblyHandler(TCPClient client, LoadAssemblyHandlerData data) {
        try {
            Assembly asm = Assembly.Load(data.Raw);
            await client.SendPacket(PacketType.AssemblyLoadResult, new AssemblyLoadResult {
                Id = data.Id,
                FullName = asm.FullName,
                Success = true
            });
        } catch (Exception e) {
            Logger.Error("Failed to load assembly {Id}: {Error}", data.Id, e.Message);
            await client.SendPacket(PacketType.AssemblyLoadResult, new AssemblyLoadResult {
                Id = data.Id,
                Success = false
            });
        }
    }

    [Handler("loadModule")]
    public static async Task LoadModuleHandler(TCPClient client, LoadModuleHandlerData data) {
        Logger.Information("Loading Module: {Raw}", data.Raw);
        try {
            byte[] raw = Encoding.UTF8.GetBytes(data.Raw);
            ModuleInfo[] loadedModules = ModuleRegistry.Load(raw);
            
            foreach (ModuleInfo loadedModule in loadedModules) {
                await client.SendPacket(PacketType.ModuleLoadResult, new ModuleLoadResult {
                    Id = data.Id,
                    ModuleId = loadedModule.Id,
                    Name = loadedModule.Name,
                    Success = true
                });
            }
        } catch (Exception e) {
            Logger.Error("Failed to load module: {Error}", e.Message);
            await client.SendPacket(PacketType.ModuleLoadResult, new ModuleLoadResult {
                Id = data.Id,
                Success = false
            });
        }
    }
}