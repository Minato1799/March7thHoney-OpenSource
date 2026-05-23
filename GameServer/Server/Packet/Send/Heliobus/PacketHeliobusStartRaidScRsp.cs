using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusStartRaidScRsp : BasePacket
{
    public PacketHeliobusStartRaidScRsp(uint retcode = 0) : base(CmdIds.HeliobusStartRaidScRsp)
    {
        SetData(new HeliobusStartRaidScRsp { Retcode = retcode });
    }
}
