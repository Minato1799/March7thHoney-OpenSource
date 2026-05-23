using March7thHoney.GameServer.Server.Packet.Send.Heliobus;
using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Recv.Heliobus;

[Opcode(CmdIds.HeliobusSnsReadCsReq)]
public class HandlerHeliobusSnsReadCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = HeliobusSnsReadCsReq.Parser.ParseFrom(data);
        connection.Player!.ActivityManager!.Heliobus.ReadPost(req.CDKEDFPEFIJ);
        await connection.SendPacket(new PacketHeliobusSnsReadScRsp(req.CDKEDFPEFIJ));
    }
}
