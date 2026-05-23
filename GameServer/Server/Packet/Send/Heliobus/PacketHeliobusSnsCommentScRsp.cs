using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusSnsCommentScRsp : BasePacket
{
    public PacketHeliobusSnsCommentScRsp(uint retcode = 0) : base(CmdIds.HeliobusSnsCommentScRsp)
    {
        SetData(new HeliobusSnsCommentScRsp { Retcode = retcode });
    }
}
