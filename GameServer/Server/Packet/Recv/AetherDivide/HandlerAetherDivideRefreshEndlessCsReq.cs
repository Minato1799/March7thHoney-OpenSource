using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.AetherDivideRefreshEndlessCsReq)]
public class HandlerAetherDivideRefreshEndlessCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        connection.Player!.ActivityManager!.AetherDivide.RefreshEndless();
        await connection.SendPacket(new PacketAetherDivideRefreshEndlessScRsp());
    }
}
