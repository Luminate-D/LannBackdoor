using System.Reflection;
using LannLogger;
using LannUtils;
using Serilog.Core;
using SystemModule.Structures;

namespace ModulesApi;

public static class ModuleRegistry {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("ModuleRegistry");
    private static readonly Dictionary<string, ModuleInfo> Modules = new();

    public static Dictionary<string, ModuleInfo> GetModules() {
        return Modules;
    }

    public static ModuleInfo? GetByName(string name) {
        return Modules.Values.SingleOrDefault(x => x!.Name == name, null);
    }

    public static void Unload(string name) {
        GetByName(name).Dispose();
        Modules.Remove(name);
    }

    public static ModuleInfo[] Load(byte[] raw) {
        return LoadByAssembly(Assembly.Load(raw));
    }

    public static ModuleInfo[] LoadByAssembly(Assembly asm) {
        Type[] modulesTypes = asm.GetTypes()
            .Where(x => x.GetCustomAttribute<Module>() != null)
            .ToArray();

        List<ModuleInfo> loadedModules = new();
        foreach (Type type in modulesTypes) {
            string moduleName = type.GetCustomAttribute<Module>()!.Name;

            if (GetByName(moduleName) != null) throw new Exception("Module is already loaded!");

            IModule instance = Activator.CreateInstance(type) as IModule;
            instance.Name = moduleName;

            MethodInfo[] handlers = type.GetMethods()
                .Where(x => x.GetCustomAttribute<Handler>() != null)
                .ToArray();

            HandlerInfo[] handlersArray = handlers.Select(x => {
                string handlerName = x.GetCustomAttribute<Handler>()!.Name;
                return new HandlerInfo(instance, handlerName, x);
            }).ToArray();

            ModuleInfo module = new(instance, moduleName, handlersArray);
            Modules.Add(moduleName, module);
            loadedModules.Add(module);

            Logger.Information("Loaded module {Name} | Handlers:", module.Name);
            foreach (HandlerInfo handlerInfo in handlersArray)
                Logger.Information("  - {Name}", handlerInfo.Name);
        }

        return loadedModules.ToArray();
    }
}