using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingRestoreGameScRsp : BasePacket
{
    public PacketSwordTrainingRestoreGameScRsp(LMBHDCFPPLL? game, uint retcode = 0) : base(CmdIds.SwordTrainingRestoreGameScRsp)
    {
        SetData(new SwordTrainingRestoreGameScRsp
        {
            Retcode = retcode,
            BMKAEFAKNFJ = game
        });
    }
}
