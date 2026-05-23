using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.GetAetherDivideChallengeInfoCsReq)]
public class HandlerGetAetherDivideChallengeInfoCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketGetAetherDivideChallengeInfoScRsp(connection.Player!));
    }
}
