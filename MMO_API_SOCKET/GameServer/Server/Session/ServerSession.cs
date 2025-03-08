using Common;
namespace Server;

public class ServerSession : Session
{
    protected sealed override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;
        var id =  BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        ServerHandler.Instance.Execute(id, buffer.Slice((int)Size.HeaderSize), this);
    }
}