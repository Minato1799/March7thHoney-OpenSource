using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.SetAetherDivideLineUpCsReq)]
public class HandlerSetAetherDivideLineUpCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetAetherDivideLineUpCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketSetAetherDivideLineUpScRsp(req.Lineup));
    }
}
