using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketAetherDivideTakeChallengeRewardScRsp : BasePacket
{
    public PacketAetherDivideTakeChallengeRewardScRsp(uint challengeId, uint retcode = 0)
        : base(CmdIds.AetherDivideTakeChallengeRewardScRsp)
    {
        SetData(new AetherDivideTakeChallengeRewardScRsp
        {
            Retcode = retcode,
            ChallengeId = challengeId
        });
    }
}
