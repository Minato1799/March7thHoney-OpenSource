using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingDailyPhaseConfirmScRsp : BasePacket
{
    public PacketSwordTrainingDailyPhaseConfirmScRsp(bool confirmed = true, uint retcode = 0) : base(CmdIds.SwordTrainingDailyPhaseConfirmScRsp)
    {
        SetData(new SwordTrainingDailyPhaseConfirmScRsp
        {
            Retcode = retcode,
            FOJHMIICBDF = confirmed
        });
    }
}
