using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.EquipAetherDividePassiveSkillCsReq)]
public class HandlerEquipAetherDividePassiveSkillCsReq : Handler
{
    public override Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = EquipAetherDividePassiveSkillCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.AetherDivide.EquipPassiveSkill(req.Slot, req.ItemId);
        // No ScRsp exists for this command; state is reflected on the next info fetch.
        return Task.CompletedTask;
    }
}
