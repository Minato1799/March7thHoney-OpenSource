using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.StartAetherDivideStageBattleCsReq)]
public class HandlerStartAetherDivideStageBattleCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketStartAetherDivideStageBattleScRsp());
    }
}
