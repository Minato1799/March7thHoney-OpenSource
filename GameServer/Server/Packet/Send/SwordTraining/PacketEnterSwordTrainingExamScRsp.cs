using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketEnterSwordTrainingExamScRsp : BasePacket
{
    public PacketEnterSwordTrainingExamScRsp(SceneBattleInfo? battleInfo = null, uint retcode = 0) : base(CmdIds.EnterSwordTrainingExamScRsp)
    {
        SetData(new EnterSwordTrainingExamScRsp
        {
            Retcode = retcode,
            BattleInfo = battleInfo
        });
    }
}
