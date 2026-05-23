using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingExamResultConfirmScRsp : BasePacket
{
    public PacketSwordTrainingExamResultConfirmScRsp(uint retcode = 0) : base(CmdIds.SwordTrainingExamResultConfirmScRsp)
    {
        SetData(new SwordTrainingExamResultConfirmScRsp { Retcode = retcode });
    }
}
