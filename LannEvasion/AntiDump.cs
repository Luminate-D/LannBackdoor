using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LannEvasion;

public class AntiDump {
    [DllImport("AntiX.dll")]
    public static extern void ErasePEHeaderFromMemory();

    [DllImport("AntiX.dll")]
    public static extern void SizeOfImage();
}
