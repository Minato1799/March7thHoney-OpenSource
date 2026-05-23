using March7thHoney.GameServer.Server.Packet.Send.SwordTraining;
using March7thHoney.Kcp;

namespace March7thHoney.GameServer.Server.Packet.Recv.SwordTraining;

[Opcode(CmdIds.SwordTrainingStoryBattleCsReq)]
public class HandlerSwordTrainingStoryBattleCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        // Story battle uses a self-contained simulated combat client-side; we acknowledge
        // the request so the flow proceeds (no SceneBattleInfo is dispatched).
        await connection.SendPacket(new PacketSwordTrainingStoryBattleScRsp());
    }
}
