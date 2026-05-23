using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.AetherDivideSpiritExpUpCsReq)]
public class HandlerAetherDivideSpiritExpUpCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var instance = connection.Player!.ActivityManager!.AetherDivide;
        // The request carries opaque consume parameters; grant a nominal spirit EXP tick.
        var level = instance.SpiritExpUp(100);
        await connection.SendPacket(new PacketAetherDivideSpiritExpUpScRsp(level));
    }
}
