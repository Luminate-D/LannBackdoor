using LannEvasion.Checks;
using LannLogger;
using Serilog.Core;

namespace LannEvasion; 

public static class Evasion {
    private static Logger Logger = LoggerFactory.CreateLogger("Evasion");
    
    private static List<Check> Checks = new() {
        new AntiDebug(),
        new Mac(),
        new AnyRun(),
        new Hardware()
    };
    
    public static async Task<bool> RunCheck() {
        Logger.Information("Running evasion check");
        return false;
    }

    public static async Task Protect() {
        Logger.Information("Injecting AntiDump");
        await AntiDump.Run();
    }
}