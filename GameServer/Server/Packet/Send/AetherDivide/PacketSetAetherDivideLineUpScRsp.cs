using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketSetAetherDivideLineUpScRsp : BasePacket
{
    public PacketSetAetherDivideLineUpScRsp(DLGFPMKPLEO? lineup, uint retcode = 0) : base(CmdIds.SetAetherDivideLineUpScRsp)
    {
        SetData(new SetAetherDivideLineUpScRsp
        {
            Retcode = retcode,
            Lineup = lineup
        });
    }
}
