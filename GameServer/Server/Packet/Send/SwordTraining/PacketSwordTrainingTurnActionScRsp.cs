using March7thHoney.Kcp;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Server.Packet.Send.SwordTraining;

public class PacketSwordTrainingTurnActionScRsp : BasePacket
{
    public PacketSwordTrainingTurnActionScRsp(IEnumerable<uint> resultIds, uint retcode = 0) : base(CmdIds.SwordTrainingTurnActionScRsp)
    {
        var rsp = new SwordTrainingTurnActionScRsp { Retcode = retcode };
        rsp.ACBIMOGNKAJ.AddRange(resultIds);
        SetData(rsp);
    }
}
