using Serilog.Sinks.SystemConsole.Themes;

namespace LannLogger;

public static class Theme {
    public static readonly Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> Data = new() {
        { ConsoleThemeStyle.Text, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.White
        } },
        { ConsoleThemeStyle.String, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkYellow
        } },
        { ConsoleThemeStyle.Number, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkBlue
        } },
        { ConsoleThemeStyle.Boolean, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkBlue
        } },
        
        { ConsoleThemeStyle.Null, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkRed
        } },
        { ConsoleThemeStyle.Invalid, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkRed
        } },
        
        { ConsoleThemeStyle.LevelDebug, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkMagenta
        } },
        { ConsoleThemeStyle.LevelInformation, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkBlue
        } },
        { ConsoleThemeStyle.LevelWarning, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkYellow
        } },
        { ConsoleThemeStyle.LevelError, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkRed
        } },
        { ConsoleThemeStyle.LevelFatal, new SystemConsoleThemeStyle {
            Foreground = ConsoleColor.DarkMagenta
        } }
    };
}