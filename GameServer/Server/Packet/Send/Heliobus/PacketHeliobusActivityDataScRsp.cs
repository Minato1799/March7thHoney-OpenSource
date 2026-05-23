using March7thHoney.GameServer.Game.Player;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusActivityDataScRsp : BasePacket
{
    public PacketHeliobusActivityDataScRsp(PlayerInstance player) : base(CmdIds.HeliobusActivityDataScRsp)
    {
        SetData(player.ActivityManager!.Heliobus.BuildActivityDataSnapshot());
    }
}
