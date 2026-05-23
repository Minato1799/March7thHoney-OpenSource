using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingStoryConfirmCsReq)]
public class HandlerSwordTrainingStoryConfirmCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingStoryConfirmCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketSwordTrainingStoryConfirmScRsp(req.MAFMCIPAIKK));
    }
}
