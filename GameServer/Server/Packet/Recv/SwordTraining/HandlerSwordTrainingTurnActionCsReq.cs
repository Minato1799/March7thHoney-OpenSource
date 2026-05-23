using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingTurnActionCsReq)]
public class HandlerSwordTrainingTurnActionCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingTurnActionCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.SwordTraining.AdvanceTurn();

        // Echo the chosen action ids back as the resolved result of the turn.
        await connection.SendPacket(new PacketSwordTrainingTurnActionScRsp(req.ACBIMOGNKAJ));
    }
}
