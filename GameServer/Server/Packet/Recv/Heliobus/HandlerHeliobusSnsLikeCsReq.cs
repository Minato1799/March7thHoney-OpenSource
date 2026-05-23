using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusSnsLikeCsReq)]
public class HandlerHeliobusSnsLikeCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = HeliobusSnsLikeCsReq.Parser.ParseFrom(data);
        var liked = connection.Player!.ActivityManager!.Heliobus.ToggleLike(req.CDKEDFPEFIJ);
        await connection.SendPacket(new PacketHeliobusSnsLikeScRsp(req.CDKEDFPEFIJ, liked));
    }
}
