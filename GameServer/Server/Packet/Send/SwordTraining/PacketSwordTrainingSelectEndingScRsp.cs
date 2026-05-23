using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingSelectEndingScRsp : BasePacket
{
    public PacketSwordTrainingSelectEndingScRsp(uint endingId, uint retcode = 0) : base(CmdIds.SwordTrainingSelectEndingScRsp)
    {
        SetData(new SwordTrainingSelectEndingScRsp
        {
            Retcode = retcode,
            AGMAHIIHJKM = endingId
        });
    }
}
