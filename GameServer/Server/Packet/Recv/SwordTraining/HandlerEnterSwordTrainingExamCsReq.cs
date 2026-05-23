using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.EnterSwordTrainingExamCsReq)]
public class HandlerEnterSwordTrainingExamCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketEnterSwordTrainingExamScRsp());
    }
}
