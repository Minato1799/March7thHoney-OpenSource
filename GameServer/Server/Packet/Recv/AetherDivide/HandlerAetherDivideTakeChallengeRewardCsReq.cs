using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.AetherDivideTakeChallengeRewardCsReq)]
public class HandlerAetherDivideTakeChallengeRewardCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = AetherDivideTakeChallengeRewardCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.AetherDivide.TakeChallengeReward(req.ChallengeId);
        await connection.SendPacket(new PacketAetherDivideTakeChallengeRewardScRsp(req.ChallengeId));
    }
}
