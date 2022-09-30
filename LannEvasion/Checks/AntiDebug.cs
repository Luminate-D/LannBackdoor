using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LannEvasion.Checks;

public class AntiDebug : Check {
[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

    [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
    private static extern bool IsDebuggerPresent();
    
    [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
    private static extern void SetLastError(uint dwErrorCode);
    
    [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
    private static extern uint GetLastError();
    
    [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
    private static extern void OutputDebugStringA(string output);
    
    public override async Task<bool> Run() {
        bool result = false;
        if (IsDebuggerPresent()) {
            SetLastError(0);
            OutputDebugStringA("Hi debugger :)");

            result = GetLastError() == 0;
        }

        result |= Debugger.IsAttached;
        
        bool isRemoteDebuggerPresent = false;
        CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isRemoteDebuggerPresent);
        
        result |= isRemoteDebuggerPresent;

        return result;
    }
}