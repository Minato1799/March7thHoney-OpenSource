using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingGiveUpGameScRsp : BasePacket
{
    public PacketSwordTrainingGiveUpGameScRsp(uint retcode = 0) : base(CmdIds.SwordTrainingGiveUpGameScRsp)
    {
        SetData(new SwordTrainingGiveUpGameScRsp { Retcode = retcode });
    }
}
