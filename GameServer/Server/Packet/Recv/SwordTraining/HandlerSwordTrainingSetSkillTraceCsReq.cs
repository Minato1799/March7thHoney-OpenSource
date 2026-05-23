using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingSetSkillTraceCsReq)]
public class HandlerSwordTrainingSetSkillTraceCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingSetSkillTraceCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.SwordTraining.SetSkillTrace(req.SkillId);
        await connection.SendPacket(new PacketSwordTrainingSetSkillTraceScRsp(req.SkillId));
    }
}
