using System.Reflection;
using Networking;
using SystemModule.Structures;

namespace ModulesApi;

public class HandlerInfo {
    public readonly string Name;
    private readonly IModule Module;
    private readonly MethodInfo Method;

    public HandlerInfo(IModule module, string name, MethodInfo method) {
        Name = name;
        Module = module;
        Method = method;
    }

    public Type GetDataType() {
        return Method.GetParameters()[1].ParameterType;
    }

    public async Task Execute(TCPClient client, object data) {
        await (Task) Method.Invoke(Module, new[] { client, data })!;
    }
}