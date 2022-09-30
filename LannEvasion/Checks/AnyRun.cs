using Utilities;

namespace LannEvasion.Checks; 

public class AnyRun : Check {
    private static readonly List<string> fileNames = new List<string>() {
        "Invoice.docx",
        "Financial_Report.ppt",
        "Incidentx.pptx",
        "Incident.pptx",
        "report.odt",
        "report.rtf"
    };

    public override async Task<bool> Run() {
        DirectoryInfo desktop = new(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        return desktop.GetFiles().Any(file => fileNames.Contains(file.Name))
               || ServiceManager.IsInstalled("VirtioSerial");
    }
}