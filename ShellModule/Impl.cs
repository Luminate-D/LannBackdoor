using LannLogger;
using LannUtils;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using ShellModule.Structures;
using ShellModule.Structures.Handlers;

namespace ShellModule;

[Module("shell")]
public class ShellModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "Shell");
    
    private static readonly IdPool _idPool = new();
    private static readonly List<ShellInstance> _shellInstances = new();

    [Handler("create")]
    public static async Task CreateHandler(TCPClient client, CreateHandlerData data) {
        ShellInstance instance = new(_idPool.NextId(), data.FileName);
        instance.OnClose += (_, _) => {
            Logger.Information("Shell with ID {ID} closed", instance.Id);
            _idPool.Dispose(instance.Id);
        };

        instance.StdOut += async (_, args) => {
            Logger.Information("Shell [{ID}] StdOut: \"{Data}\"", args.Data);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Message,
                Data = new OutputMessage {
                    Id = instance.Id,
                    Error = false,
                    Data = args.Data
                }
            });
        };
        
        instance.StdErr += async (_, args) => {
            Logger.Information("Shell [{ID}] StdErr: \"{Data}\"", args.Data);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Message,
                Data = new OutputMessage {
                    Id = instance.Id,
                    Error = true,
                    Data = args.Data
                }
            });
        };

        try {
            instance.Start();
            _shellInstances.Add(instance);

            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new CreateResult {
                    Id = instance.Id,
                    Success = true
                }
            });
        } catch (Exception error) {
            Logger.Error("Failed to start shell: {Error}", error);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new CreateResult {
                    Success = false
                }
            });
            
            _idPool.Dispose(instance.Id);
        }
    }

    [Handler("write")]
    public static async Task WriteHandler(TCPClient client, WriteHandlerData data) {
        ShellInstance instance = _shellInstances.Find(instance => instance.Id == data.Id);
        if (instance == null) {
            Logger.Error("Unknown shell: {Id}", data.Id);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new WriteResult {
                    Id = data.Id,
                    Success = false,
                    ErrorCode = 0
                }
            });
        }
        
        try {
            await instance.Write(data.Data);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new WriteResult {
                    Id = data.Id,
                    Success = true
                }
            });
        } catch (Exception error) {
            Logger.Error("Failed to Write to Shell {ID}: {Error}", data.Id, error);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new WriteResult {
                    Id = data.Id,
                    Success = false,
                    ErrorCode = 1
                }
            });
        }
    }

    [Handler("close")]
    public static async Task CloseHandler(TCPClient client, CloseHandlerData data) {
        ShellInstance instance = _shellInstances.Find(instance => instance.Id == data.Id);
        if (instance == null) {
            Logger.Error("Unknown shell: {Id}", data.Id);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new CloseResult {
                    Id = data.Id,
                    Success = false,
                    ErrorCode = 0
                }
            });
        }

        try {
            instance.Close();
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new CloseResult {
                    Id = data.Id,
                    Success = true
                }
            });
        } catch (Exception error) {
            Logger.Error("Failed to Close Shell {ID}: {Error}", data.Id, error);
            await client.SendPacket(new ClientPacket {
                Type = PacketType.Callback,
                Data = new CloseResult {
                    Id = data.Id,
                    Success = false,
                    ErrorCode = 1
                }
            });
        }
    }
}