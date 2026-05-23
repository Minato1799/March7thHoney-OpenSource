using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketSwitchAetherDivideLineUpSlotScRsp : BasePacket
{
    public PacketSwitchAetherDivideLineUpSlotScRsp(uint slot, uint retcode = 0) : base(CmdIds.SwitchAetherDivideLineUpSlotScRsp)
    {
        SetData(new SwitchAetherDivideLineUpSlotScRsp
        {
            Retcode = retcode,
            FNCINGFDLPA = slot
        });
    }
}
