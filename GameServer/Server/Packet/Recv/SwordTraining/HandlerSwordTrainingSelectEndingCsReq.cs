using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingSelectEndingCsReq)]
public class HandlerSwordTrainingSelectEndingCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingSelectEndingCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketSwordTrainingSelectEndingScRsp(req.AGMAHIIHJKM));
    }
}
