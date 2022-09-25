namespace ModulesApi;

[AttributeUsage(AttributeTargets.Class)]
public sealed class Module : Attribute {
    public string Name { get; }

    public Module(string name) {
        Name = name;
    }
}