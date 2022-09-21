namespace Networking.Packets; 

public enum PacketType {
    Pong,
    Ready, Verified,
    
    ModuleLoadResult,
    AssemblyLoadResult
}
