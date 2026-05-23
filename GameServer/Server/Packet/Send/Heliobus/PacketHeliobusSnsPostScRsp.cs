using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusSnsPostScRsp : BasePacket
{
    public PacketHeliobusSnsPostScRsp(uint retcode = 0) : base(CmdIds.HeliobusSnsPostScRsp)
    {
        SetData(new HeliobusSnsPostScRsp { Retcode = retcode });
    }
}
