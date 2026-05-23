using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingStoryConfirmScRsp : BasePacket
{
    public PacketSwordTrainingStoryConfirmScRsp(uint storyId, uint retcode = 0) : base(CmdIds.SwordTrainingStoryConfirmScRsp)
    {
        SetData(new SwordTrainingStoryConfirmScRsp
        {
            MAFMCIPAIKK = storyId,
            Retcode = retcode
        });
    }
}
