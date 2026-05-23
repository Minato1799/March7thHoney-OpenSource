using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusSnsLikeScRsp : BasePacket
{
    public PacketHeliobusSnsLikeScRsp(uint snsId, bool liked, uint retcode = 0) : base(CmdIds.HeliobusSnsLikeScRsp)
    {
        SetData(new HeliobusSnsLikeScRsp
        {
            Retcode = retcode,
            CDKEDFPEFIJ = snsId,
            BOLCAEPIHJH = liked
        });
    }
}
