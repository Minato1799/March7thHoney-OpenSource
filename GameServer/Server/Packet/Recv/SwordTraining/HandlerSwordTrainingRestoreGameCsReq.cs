using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingRestoreGameCsReq)]
public class HandlerSwordTrainingRestoreGameCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var game = connection.Player!.ActivityManager!.SwordTraining.RestoreGame();
        await connection.SendPacket(new PacketSwordTrainingRestoreGameScRsp(game));
    }
}
