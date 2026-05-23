using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingSetSkillTraceScRsp : BasePacket
{
    public PacketSwordTrainingSetSkillTraceScRsp(uint skillId, uint retcode = 0) : base(CmdIds.SwordTrainingSetSkillTraceScRsp)
    {
        SetData(new SwordTrainingSetSkillTraceScRsp
        {
            SkillId = skillId,
            Retcode = retcode
        });
    }
}
