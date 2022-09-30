using System.Net.NetworkInformation;

namespace LannEvasion.Checks; 

public class Mac : Check {
    private static readonly List<string> adresses = new List<string>() {
        "000C29",
        "001C14",
        "005056",
        "000569",
        "080027"
    };

    public override async Task<bool> Run() {
        return NetworkInterface.GetAllNetworkInterfaces().Any((adapter) => {
            return adresses.Any(address =>
                adapter.GetPhysicalAddress().ToString().ToLower().StartsWith(address.ToLower()));
        });
    }
}