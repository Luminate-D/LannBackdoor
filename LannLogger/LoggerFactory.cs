using LannConstants;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LannLogger; 

public static class LoggerFactory {
    public static Serilog.Core.Logger CreateLogger(string which) {
        LoggerConfiguration cfg = new LoggerConfiguration()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [" + which + "] {Message:lj}{NewLine}{Exception}",
                theme: new SystemConsoleTheme(Theme.Data));

        if (Constants.DEBUG) cfg.MinimumLevel.Verbose();
        else cfg.MinimumLevel.Information();

        return cfg.CreateLogger();
    }
}