using System.Reflection;

namespace Modules;

public class ModuleRegistry {
    public static ModuleRegistry Instance = new();
    private static Dictionary<int, object> _modules = new();
    
    private ModuleRegistry() {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) => {
            Console.WriteLine("AssemblyResolve: " + args.Name);
            return args.RequestingAssembly;
        };
    }
    
    public void LoadModule(string path) {
        Assembly asm = Assembly.Load(path);
        Console.WriteLine("Loaded assembly: " + asm.FullName);
    }
}