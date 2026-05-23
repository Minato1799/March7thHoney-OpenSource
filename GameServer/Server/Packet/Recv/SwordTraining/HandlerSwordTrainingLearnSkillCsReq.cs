using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingLearnSkillCsReq)]
public class HandlerSwordTrainingLearnSkillCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingLearnSkillCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.SwordTraining.LearnSkill(req.SkillId);
        await connection.SendPacket(new PacketSwordTrainingLearnSkillScRsp(req.SkillId));
    }
}
