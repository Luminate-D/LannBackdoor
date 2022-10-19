using Networking;
using Networking.Packets;
using Networking.Structures;

namespace SystemModule.Structures;

public abstract class IModule : IDisposable {
    public string Name;
    public void Dispose() { }

    protected async Task Callback(TCPClient client, string handler, object data) {
        await client.SendPacket(new ClientPacket {
            ModuleName = Name,
            HandlerName = handler,
            Type = PacketType.Callback,
            Data = data
        });
    }

    protected async Task Message(TCPClient client, object data) {
        await client.SendPacket(new ClientPacket {
            ModuleName = Name,
            Type = PacketType.Message,
            Data = data
        });
    }
}