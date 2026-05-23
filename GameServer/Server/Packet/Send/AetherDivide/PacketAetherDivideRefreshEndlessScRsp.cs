using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketAetherDivideRefreshEndlessScRsp : BasePacket
{
    public PacketAetherDivideRefreshEndlessScRsp(uint retcode = 0) : base(CmdIds.AetherDivideRefreshEndlessScRsp)
    {
        SetData(new AetherDivideRefreshEndlessScRsp { Retcode = retcode });
    }
}
