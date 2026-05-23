using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusEnterBattleCsReq)]
public class HandlerHeliobusEnterBattleCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = HeliobusEnterBattleCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketHeliobusEnterBattleScRsp(req.EventId));
    }
}
