namespace ModulesApi; 

[AttributeUsage(AttributeTargets.Class)]
public class Module : Attribute {
    public virtual string Name { get; }

    public Module(string name) {
        Name = name;
    }
}