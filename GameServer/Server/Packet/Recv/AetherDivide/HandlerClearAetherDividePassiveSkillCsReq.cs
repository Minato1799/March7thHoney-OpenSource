using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.ClearAetherDividePassiveSkillCsReq)]
public class HandlerClearAetherDividePassiveSkillCsReq : Handler
{
    public override Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ClearAetherDividePassiveSkillCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.AetherDivide.ClearPassiveSkill(req.Slot);
        return Task.CompletedTask;
    }
}
