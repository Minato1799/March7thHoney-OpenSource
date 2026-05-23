using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingMarkEndingViewedCsReq)]
public class HandlerSwordTrainingMarkEndingViewedCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var instance = connection.Player!.ActivityManager!.SwordTraining;
        // The request carries no fields; mark the most recently selected ending as viewed
        // by flagging the current story line so the gallery reflects progress.
        instance.MarkEndingViewed(instance.Data.CurGameStoryLineId);
        await connection.SendPacket(new PacketSwordTrainingMarkEndingViewedScRsp());
    }
}
