using System.Reflection;
using Networking;

namespace ModulesApi;

public class HandlerInfo {
    public readonly int Id;
    public readonly string Name;
    private readonly MethodInfo _method;

    public HandlerInfo(int id, string name, MethodInfo method) {
        Id = id;
        Name = name;
        _method = method;
    }

    public Type GetDataType() {
        return _method.GetParameters()[1].ParameterType;
    }

    public async Task Execute(TCPClient client, object data) {
        await (Task) _method.Invoke(null, new[] { client, data })!;
    }
}