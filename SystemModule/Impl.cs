using LannLogger;
using ModulesApi;
using Networking;
using Serilog.Core;

namespace SystemModule;

[Module("system")]
public class SystemModuleImpl {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("Module", "System");
    
    [Handler("ping")]
    public static async Task PingHandler(TCPClient client, IPingHandlerData data) {
        Logger.Information("Ping handler called (hey from module)");
    }
}

public class IPingHandlerData { }