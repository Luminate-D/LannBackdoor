using LannConstants;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace LannLogger;

public static class LoggerFactory {
    public static Serilog.Core.Logger CreateLogger(params string[] which) {
        string logWhich = string.Join(' ', which.Select(x => $"[{x}]"));
        LoggerConfiguration cfg = new LoggerConfiguration()
                                  .WriteTo.Console(
                                      outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] " + logWhich +
                                                      " {Message:lj}{NewLine}{Exception}",
                                      theme: new SystemConsoleTheme(Theme.Data));

        if (Constants.Debug) cfg.MinimumLevel.Verbose();
        else cfg.MinimumLevel.Information();

        return cfg.CreateLogger();
    }
}