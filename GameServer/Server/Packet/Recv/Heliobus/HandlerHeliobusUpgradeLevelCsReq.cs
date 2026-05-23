using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusUpgradeLevelCsReq)]
public class HandlerHeliobusUpgradeLevelCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var level = connection.Player!.ActivityManager!.Heliobus.UpgradeLevel();
        await connection.SendPacket(new PacketHeliobusUpgradeLevelScRsp(level));
    }
}
