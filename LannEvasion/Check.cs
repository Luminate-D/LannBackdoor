namespace LannEvasion;

public abstract class Check {
    public abstract Task<bool> Run();
}