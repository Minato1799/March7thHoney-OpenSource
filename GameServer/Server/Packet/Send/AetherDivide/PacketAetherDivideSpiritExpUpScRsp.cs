using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketAetherDivideSpiritExpUpScRsp : BasePacket
{
    public PacketAetherDivideSpiritExpUpScRsp(uint level, uint retcode = 0) : base(CmdIds.AetherDivideSpiritExpUpScRsp)
    {
        SetData(new AetherDivideSpiritExpUpScRsp
        {
            Retcode = retcode,
            FIKLLOCJBGN = level
        });
    }
}
