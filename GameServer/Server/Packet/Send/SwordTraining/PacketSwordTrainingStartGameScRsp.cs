using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingStartGameScRsp : BasePacket
{
    public PacketSwordTrainingStartGameScRsp(LMBHDCFPPLL? game, uint retcode = 0) : base(CmdIds.SwordTrainingStartGameScRsp)
    {
        SetData(new SwordTrainingStartGameScRsp
        {
            Retcode = retcode,
            BMKAEFAKNFJ = game
        });
    }
}
