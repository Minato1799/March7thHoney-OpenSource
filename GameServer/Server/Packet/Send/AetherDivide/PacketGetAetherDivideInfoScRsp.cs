using March7thHoney.GameServer.Game.Player;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketGetAetherDivideInfoScRsp : BasePacket
{
    public PacketGetAetherDivideInfoScRsp(PlayerInstance player) : base(CmdIds.GetAetherDivideInfoScRsp)
    {
        SetData(player.ActivityManager!.AetherDivide.BuildInfoSnapshot());
    }
}
