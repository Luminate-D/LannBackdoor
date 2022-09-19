using System.Reflection;
using LannLogger;
using LannUtils;
using Serilog.Core;

namespace ModulesApi;

public class ModuleRegistry {
    private static readonly Logger Logger = LoggerFactory.CreateLogger("ModuleRegistry");
    private static readonly IdPool ModuleIdPool = new();
    private static readonly Dictionary<int, ModuleInfo> Modules = new();

    public static ModuleInfo? Get(int id) {
        KeyValuePair<int, ModuleInfo?> kvPair = Modules.SingleOrDefault(
            pair => pair.Key == id,
            new KeyValuePair<int, ModuleInfo>())!;

        return kvPair.Value;
    }

    public static ModuleInfo? GetByName(string name) {
        return Modules.Values.SingleOrDefault(x => x!.Name == name, null);
    }

    public static void Unload(int id) {
        ModuleIdPool.Dispose(id);
        Modules.Remove(id);
    }

    public static void Load(byte[] raw) {
        LoadByAssembly(Assembly.Load(raw));
    }
    
    public static void LoadByAssembly(Assembly asm) {
        Type[] modulesTypes = asm.GetTypes()
            .Where(x => x.GetCustomAttribute<Module>() != null)
            .ToArray();

        foreach (Type type in modulesTypes) {
            string moduleName = type.GetCustomAttribute<Module>()!.Name;
            int id = ModuleIdPool.NextId();

            if (GetByName(moduleName) != null) throw new Exception("Module is already loaded!");
            
            IdPool handlerIdPool = new();
            MethodInfo[] handlers = type.GetMethods()
                .Where(x => x.GetCustomAttribute<Handler>() != null)
                .ToArray();

            HandlerInfo[] handlersArray = handlers.Select(x => {
                string handlerName = x.GetCustomAttribute<Handler>()!.Name;
                int id = handlerIdPool.NextId();
                return new HandlerInfo(id, handlerName, x);
            }).ToArray();

            ModuleInfo module = new(id, moduleName, handlersArray);
            Modules.Add(id, module);
            
            Logger.Information("Loaded module {Name} (ID: {Id}) | Handlers:",
                module.Name, module.Id);
            foreach (HandlerInfo handlerInfo in handlersArray) {
                Logger.Information("  - {Name} (ID: {Id})",
                    handlerInfo.Name, handlerInfo.Id);
            }
        }
    }
}