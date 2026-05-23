using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingStartGameCsReq)]
public class HandlerSwordTrainingStartGameCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingStartGameCsReq.Parser.ParseFrom(data);
        var game = connection.Player!.ActivityManager!.SwordTraining.StartGame(req.GameStoryLineId);
        await connection.SendPacket(new PacketSwordTrainingStartGameScRsp(game));
    }
}
