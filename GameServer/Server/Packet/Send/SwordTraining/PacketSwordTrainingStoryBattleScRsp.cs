using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingStoryBattleScRsp : BasePacket
{
    public PacketSwordTrainingStoryBattleScRsp(SceneBattleInfo? battleInfo = null, uint retcode = 0) : base(CmdIds.SwordTrainingStoryBattleScRsp)
    {
        SetData(new SwordTrainingStoryBattleScRsp
        {
            Retcode = retcode,
            BattleInfo = battleInfo
        });
    }
}
