using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.Heliobus;

public class PacketHeliobusUpgradeLevelScRsp : BasePacket
{
    public PacketHeliobusUpgradeLevelScRsp(uint level, uint retcode = 0) : base(CmdIds.HeliobusUpgradeLevelScRsp)
    {
        SetData(new HeliobusUpgradeLevelScRsp
        {
            Retcode = retcode,
            Level = level
        });
    }
}
