using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusSnsPostCsReq)]
public class HandlerHeliobusSnsPostCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        connection.Player!.ActivityManager!.Heliobus.CreatePost();
        await connection.SendPacket(new PacketHeliobusSnsPostScRsp());
    }
}
