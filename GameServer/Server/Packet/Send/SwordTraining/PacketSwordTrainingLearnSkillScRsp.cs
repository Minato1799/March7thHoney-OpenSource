using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingLearnSkillScRsp : BasePacket
{
    public PacketSwordTrainingLearnSkillScRsp(uint skillId, uint retcode = 0) : base(CmdIds.SwordTrainingLearnSkillScRsp)
    {
        SetData(new SwordTrainingLearnSkillScRsp
        {
            SkillId = skillId,
            Retcode = retcode
        });
    }
}
