using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.StartAetherDivideSceneBattleCsReq)]
public class HandlerStartAetherDivideSceneBattleCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = StartAetherDivideSceneBattleCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketStartAetherDivideSceneBattleScRsp(req.CastEntityId));
    }
}
