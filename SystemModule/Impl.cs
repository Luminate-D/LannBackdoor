﻿using System.Reflection;
using LannLogger;
using ModulesApi;
using Networking;
using Networking.Packets;
using Serilog.Core;
using SystemModule.Structures;

namespace SystemModule;

[ModulesApi.Module("system")]
public class SystemModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "System");
    
    [Handler("ping")]
    public static async Task PingHandler(TCPClient client, object data) {
        await client.SendPacket(PacketType.Pong, new { });
    }
    
    [Handler("verify")]
    public static async Task VerifyHandler(TCPClient client, VerifyHandlerData data) {
        if (client.IsVerified) throw new Exception("Verify packet received twice or more.");

        client.IsVerified = Signing.VerifySigned(data.Timestamp, data.Signature);
        if (!client.IsVerified) {
            Logger.Fatal("Failed to verify signature, data: {Raw}, signature: {Signature}",
                data.Timestamp,
                data.Signature);
            client.Dispose();
            return;
        }

        await client.SendPacket(PacketType.Verified, new { });
    }

    [Handler("loadAssembly")]
    public static async Task LoadAssemblyHandler(TCPClient client, LoadAssemblyHandlerData data) {
        if (!client.IsVerified) {
            client.Dispose();
            return;
        }
        
        try {
            byte[] raw = Convert.FromBase64String(data.Raw);
            Assembly asm = Assembly.Load(raw);
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
        if (!client.IsVerified) {
            client.Dispose();
            return;
        }

        try {
            byte[] raw = Convert.FromBase64String(data.Raw);
            Logger.Information("Loading Module: {Length} bytes", raw.Length);
            
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