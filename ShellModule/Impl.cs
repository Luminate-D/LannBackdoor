using LannLogger;
using LannUtils;
using ModulesApi;
using Networking;
using Networking.Packets;
using Networking.Structures;
using Serilog.Core;
using ShellModule.Structures;
using ShellModule.Structures.Handlers;
using SystemModule.Structures;

namespace ShellModule;

[Module("shell")]
public class ShellModuleImpl : IModule {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "Shell");

    private readonly IdPool _idPool;
    private readonly List<ShellInstance> _shellInstances;

    private ShellModuleImpl() {
        _idPool = new IdPool();
        _shellInstances = new List<ShellInstance>();
    }

    [Handler("create")]
    public async Task CreateHandler(TCPClient client, CreateHandlerData data) {
        ShellInstance instance = new(_idPool.NextId(), data.FileName);
        CreateResult result = new();

        instance.OnClose += (_, _) => {
            Logger.Information("Shell with ID {ID} closed", instance.Id);
            _idPool.Dispose(instance.Id);
        };

        instance.StdOut += async (_, args) => {
            Logger.Information("Shell [{ID}] StdOut: \"{Data}\"", args.Data);
            await Message(client, new OutputMessage {
                Id = instance.Id,
                Error = false,
                Data = args.Data
            });
        };

        instance.StdErr += async (_, args) => {
            Logger.Information("Shell [{ID}] StdErr: \"{Data}\"", args.Data);
            await Message(client, new OutputMessage {
                Id = instance.Id,
                Error = true,
                Data = args.Data
            });
        };

        try {
            instance.Start();
            _shellInstances.Add(instance);

            result.Success = true;
            result.Id = instance.Id;
        } catch (Exception error) {
            Logger.Error("Failed to start shell: {Error}", error);
            result.Success = false;
            result.Error = error.Message;

            _idPool.Dispose(instance.Id);
        }

        await Callback(client, "create", result);
    }

    [Handler("write")]
    public async Task WriteHandler(TCPClient client, WriteHandlerData data) {
        ShellInstance instance = _shellInstances.Find(instance => instance.Id == data.Id);
        WriteResult result = new();

        if (instance == null) {
            Logger.Error("Unknown shell: {Id}", data.Id);
            result.Success = false;
            result.Id = data.Id;
            result.Error = "Unknown shell";

            Callback(client, "write", result);
            return;
        }

        try {
            await instance.Write(data.Data);
            result.Success = true;
            result.Id = data.Id;
        } catch (Exception error) {
            Logger.Error("Failed to Write to Shell {ID}: {Error}", data.Id, error);
            result.Success = false;
            result.Error = error.Message;
            result.Id = data.Id;
        }

        await Callback(client, "write", result);
    }

    [Handler("close")]
    public async Task CloseHandler(TCPClient client, CloseHandlerData data) {
        ShellInstance instance = _shellInstances.Find(instance => instance.Id == data.Id);
        CloseResult result = new();

        if (instance == null) {
            Logger.Error("Unknown shell: {Id}", data.Id);
            result.Id = data.Id;
            result.Success = false;
            result.Error = "Unknown shell";

            await Callback(client, "close", result);
            return;
        }

        try {
            instance.Close();
            result.Id = data.Id;
            result.Success = true;
        } catch (Exception error) {
            Logger.Error("Failed to Close Shell {ID}: {Error}", data.Id, error);
            result.Success = false;
            result.Id = data.Id;
            result.Error = error.Message;
        }

        await Callback(client, "close", result);
    }
}