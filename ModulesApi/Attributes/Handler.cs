namespace ModulesApi;

[AttributeUsage(AttributeTargets.Method)]
public sealed class Handler : Attribute {
    public string Name { get; }

    public Handler(string name) {
        Name = name;
    }
}