using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusStartRaidCsReq)]
public class HandlerHeliobusStartRaidCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketHeliobusStartRaidScRsp());
    }
}
