namespace ModulesApi;

public class ModuleInfo {
    public readonly int Id;
    public readonly string Name;
    private readonly HandlerInfo[] _handlers;

    public ModuleInfo(int id, string name, HandlerInfo[] handlers) {
        Id = id;
        Name = name;
        _handlers = handlers;
    }

    public HandlerInfo? GetHandler(int id) {
        return _handlers.SingleOrDefault(x => x!.Id == id, null);
    }
}