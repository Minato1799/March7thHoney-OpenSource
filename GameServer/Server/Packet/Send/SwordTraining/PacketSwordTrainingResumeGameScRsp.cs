using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingResumeGameScRsp : BasePacket
{
    public PacketSwordTrainingResumeGameScRsp(LMBHDCFPPLL? game, uint retcode = 0) : base(CmdIds.SwordTrainingResumeGameScRsp)
    {
        SetData(new SwordTrainingResumeGameScRsp
        {
            Retcode = retcode,
            BMKAEFAKNFJ = game
        });
    }
}
