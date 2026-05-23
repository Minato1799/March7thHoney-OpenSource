using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingGiveUpGameCsReq)]
public class HandlerSwordTrainingGiveUpGameCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        connection.Player!.ActivityManager!.SwordTraining.GiveUpGame();
        await connection.SendPacket(new PacketSwordTrainingGiveUpGameScRsp());
    }
}
