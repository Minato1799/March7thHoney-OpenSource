using March7thHoney.GameServer.Game.Player;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketGetAetherDivideChallengeInfoScRsp : BasePacket
{
    public PacketGetAetherDivideChallengeInfoScRsp(PlayerInstance player) : base(CmdIds.GetAetherDivideChallengeInfoScRsp)
    {
        SetData(player.ActivityManager!.AetherDivide.BuildChallengeInfoSnapshot());
    }
}
