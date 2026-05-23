using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.AetherDivide;

public class PacketStartAetherDivideSceneBattleScRsp : BasePacket
{
    public PacketStartAetherDivideSceneBattleScRsp(uint castEntityId = 0, uint retcode = 0)
        : base(CmdIds.StartAetherDivideSceneBattleScRsp)
    {
        SetData(new StartAetherDivideSceneBattleScRsp
        {
            Retcode = retcode,
            CastEntityId = castEntityId
        });
    }
}
