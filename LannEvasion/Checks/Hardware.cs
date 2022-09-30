using System.Management;

namespace LannEvasion.Checks;

#pragma warning disable CA1416

public class Hardware : Check {
    public override async Task<bool> Run() {
        using (ManagementObjectSearcher
               searcher = new("select * from Win32_VideoController")) {
            foreach (ManagementBaseObject o in searcher.Get()) {
                ManagementObject obj = (ManagementObject) o;
                if (obj["Name"].ToString()!.ToLower().Contains("vmware")) return true;
                if (obj["Name"].ToString()!.ToLower().Contains("vbox")) return true;
            }
        }
        //if (computer.Manufacturer.ToLower().Contains("microsoft corporation") && computer.Model.ToLower().Contains("virtual")) return true;
        //if (computer.Manufacturer.ToLower().Contains("vmware")) return true;
        //if (computer.Model.ToLower().Contains("virtualbox")) return true;

        return false;
    }
}

#pragma warning restore CA1416