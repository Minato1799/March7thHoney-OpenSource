using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingResumeGameCsReq)]
public class HandlerSwordTrainingResumeGameCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingResumeGameCsReq.Parser.ParseFrom(data);
        var game = connection.Player!.ActivityManager!.SwordTraining.ResumeGame(req.GameStoryLineId);
        await connection.SendPacket(new PacketSwordTrainingResumeGameScRsp(game));
    }
}
