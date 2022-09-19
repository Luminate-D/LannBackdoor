namespace ModulesApi; 

[AttributeUsage(AttributeTargets.Method)]
public class Handler : Attribute {
    public virtual string Name { get; }
    
    public Handler(string name) {
        Name = name;
    }
}