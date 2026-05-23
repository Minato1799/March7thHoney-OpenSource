using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingDailyPhaseConfirmCsReq)]
public class HandlerSwordTrainingDailyPhaseConfirmCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwordTrainingDailyPhaseConfirmCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.SwordTraining.ConfirmDailyPhase((uint)req.BFPFDMGMCAI);
        await connection.SendPacket(new PacketSwordTrainingDailyPhaseConfirmScRsp());
    }
}
