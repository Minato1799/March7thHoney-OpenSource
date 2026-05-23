using March7thHoney.GameServer.Game.Player;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketGetSwordTrainingDataScRsp : BasePacket
{
    public PacketGetSwordTrainingDataScRsp(PlayerInstance player) : base(CmdIds.GetSwordTrainingDataScRsp)
    {
        SetData(player.ActivityManager!.SwordTraining.BuildDataSnapshot());
    }
}
