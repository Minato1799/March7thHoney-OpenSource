using March7thHoney.GameServer.Server.Packet.Send.AetherDivide;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.AetherDivide;

[Opcode(CmdIds.SwitchAetherDivideLineUpSlotCsReq)]
public class HandlerSwitchAetherDivideLineUpSlotCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SwitchAetherDivideLineUpSlotCsReq.Parser.ParseFrom(data);
        await connection.SendPacket(new PacketSwitchAetherDivideLineUpSlotScRsp(req.FNCINGFDLPA));
    }
}
