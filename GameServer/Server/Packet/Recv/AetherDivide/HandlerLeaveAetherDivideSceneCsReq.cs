using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.LeaveAetherDivideSceneCsReq)]
public class HandlerLeaveAetherDivideSceneCsReq : Handler
{
    public override Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        // Leaving the Aetherium Wars scene has no dedicated ScRsp; acknowledge silently.
        return Task.CompletedTask;
    }
}
