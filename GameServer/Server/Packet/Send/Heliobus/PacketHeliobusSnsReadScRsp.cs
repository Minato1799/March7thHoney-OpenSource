using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusSnsReadScRsp : BasePacket
{
    public PacketHeliobusSnsReadScRsp(uint snsId, uint retcode = 0) : base(CmdIds.HeliobusSnsReadScRsp)
    {
        SetData(new HeliobusSnsReadScRsp
        {
            Retcode = retcode,
            CDKEDFPEFIJ = snsId
        });
    }
}
