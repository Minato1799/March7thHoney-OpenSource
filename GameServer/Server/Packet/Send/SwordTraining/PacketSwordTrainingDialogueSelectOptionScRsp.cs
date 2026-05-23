using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingDialogueSelectOptionScRsp : BasePacket
{
    public PacketSwordTrainingDialogueSelectOptionScRsp(uint retcode = 0) : base(CmdIds.SwordTrainingDialogueSelectOptionScRsp)
    {
        SetData(new SwordTrainingDialogueSelectOptionScRsp { Retcode = retcode });
    }
}
