using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingMarkEndingViewedScRsp : BasePacket
{
    public PacketSwordTrainingMarkEndingViewedScRsp(uint retcode = 0) : base(CmdIds.SwordTrainingMarkEndingViewedScRsp)
    {
        SetData(new SwordTrainingMarkEndingViewedScRsp { Retcode = retcode });
    }
}
