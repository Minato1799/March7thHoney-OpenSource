using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketStartAetherDivideChallengeBattleScRsp : BasePacket
{
    public PacketStartAetherDivideChallengeBattleScRsp(uint retcode = 0) : base(CmdIds.StartAetherDivideChallengeBattleScRsp)
    {
        SetData(new StartAetherDivideChallengeBattleScRsp { Retcode = retcode });
    }
}
