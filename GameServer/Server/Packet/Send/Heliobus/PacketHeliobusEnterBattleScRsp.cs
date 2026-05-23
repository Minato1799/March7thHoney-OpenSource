using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusEnterBattleScRsp : BasePacket
{
    public PacketHeliobusEnterBattleScRsp(uint eventId, uint retcode = 0) : base(CmdIds.HeliobusEnterBattleScRsp)
    {
        SetData(new HeliobusEnterBattleScRsp
        {
            Retcode = retcode,
            EventId = eventId
        });
    }
}
