namespace Networking.Packets;

public enum PacketType {
    Ping,
    Pong,
    Ready,
    Verified,

    ModuleLoadResult,
    AssemblyLoadResult,

    Callback,
    Message
}