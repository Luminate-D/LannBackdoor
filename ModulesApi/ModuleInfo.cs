using SystemModule.Structures;

namespace ModulesApi;

public class ModuleInfo {
    public readonly string Name;
    private readonly HandlerInfo[] Handlers;
    private readonly IModule Instance;

    public ModuleInfo(IModule instance, string name, HandlerInfo[] handlers) {
        Instance = instance;
        Name = name;
        Handlers = handlers;
    }

    public void Dispose() {
        Instance.Dispose();
    }

    public HandlerInfo? GetHandler(string name) {
        return Handlers.SingleOrDefault(x => x!.Name == name, null);
    }
}