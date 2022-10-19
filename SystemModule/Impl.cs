using System.Reflection;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using SystemModule.Structures;

namespace SystemModule;

[ModulesApi.Module("system")]
public class SystemModuleImpl : IModule {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "System");

    [Handler("ping")]
    public async Task PingHandler(TCPClient client, EmptyHandlerData data) {
        await client.SendPacket(new ClientPacket { Type = PacketType.Pong });
    }

    [Handler("verify")]
    public async Task VerifyHandler(TCPClient client, VerifyHandlerData data) {
        if (client.IsVerified) throw new Exception("Verify packet received twice or more.");

        if (Signing.VerifySigned(data.Timestamp, data.Signature)) client.Id = data.Id;
        if (!client.IsVerified) {
            Logger.Fatal("Failed to verify signature, data: {Raw}, signature: {Signature}",
                data.Timestamp,
                data.Signature);
            client.Dispose();
            return;
        }

        await client.SendPacket(new ClientPacket { Type = PacketType.Verified });
    }

    [Handler("loadAssembly")]
    public async Task LoadAssemblyHandler(TCPClient client, LoadAssemblyHandlerData data) {
        if (!client.IsVerified) {
            client.Dispose();
            return;
        }

        try {
            byte[] raw = Convert.FromBase64String(data.Raw);
            Assembly asm = Assembly.Load(raw);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.AssemblyLoadResult,
                Data = new AssemblyLoadResult {
                    Id = data.Id,
                    FullName = asm.FullName,
                    Success = true
                }
            });
        } catch (Exception e) {
            Logger.Error("Failed to load assembly {Id}: {Error}", data.Id, e.Message);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.AssemblyLoadResult,
                Data = new AssemblyLoadResult {
                    Id = data.Id,
                    Error = e.Message,
                    Success = false
                }
            });
        }
    }

    [Handler("loadModule")]
    public async Task LoadModuleHandler(TCPClient client, LoadModuleHandlerData data) {
        if (!client.IsVerified) {
            client.Dispose();
            return;
        }

        try {
            byte[] raw = Convert.FromBase64String(data.Raw);
            Logger.Information("Loading Module: {Length} bytes", raw.Length);

            ModuleInfo[] loadedModules = ModuleRegistry.Load(raw);

            foreach (ModuleInfo loadedModule in loadedModules)
                await client.SendPacket(new ClientPacket {
                    Type = PacketType.ModuleLoadResult,
                    Data = new ModuleLoadResult {
                        Id = data.Id,
                        Name = loadedModule.Name,
                        Success = true
                    }
                });
        } catch (Exception e) {
            Logger.Error("Failed to load module: {Error}", e.Message);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.ModuleLoadResult,
                Data = new ModuleLoadResult {
                    Id = data.Id,
                    Error = e.Message,
                    Success = false
                }
            });
        }
    }

    [Handler("listModules")]
    public async Task ListModulesHandler(TCPClient client, EmptyHandlerData data) {
        Dictionary<string, ModuleInfo> modules = ModuleRegistry.GetModules();

        await client.SendPacket(new ClientPacket {
            Type = PacketType.Callback,
            Data = new ListModulesResult {
                List = modules.Select(keyPair => new ListedModule {
                    Name = keyPair.Value.Name
                }).ToArray()
            }
        });
    }
}