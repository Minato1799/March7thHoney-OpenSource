using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketStartAetherDivideStageBattleScRsp : BasePacket
{
    public PacketStartAetherDivideStageBattleScRsp(uint retcode = 0) : base(CmdIds.StartAetherDivideStageBattleScRsp)
    {
        SetData(new StartAetherDivideStageBattleScRsp { Retcode = retcode });
    }
}
