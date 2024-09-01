using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;

namespace GameServer;
//supersocket lite -> nuget에 존재함 


public class EFBinaryRequestInfo : BinaryRequestInfo
{
    //set을 정의하지 않으면 ? ..
    public short TotalSize { get; }
    
    public short PacketID { get; }

    public const int HEADER_SIZE = 4;
    
    public EFBinaryRequestInfo(short totalSize, short packetID, byte[] body) : base(null, body)
    {
        TotalSize = totalSize;
        PacketID = packetID;
    }
}

public class ReceiveFilter : FixedHeaderReceiveFilter<EFBinaryRequestInfo>
{
    public ReceiveFilter(int headerSize) : base(headerSize)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if(BitConverter.IsLittleEndian)
            Array.Reverse(header, offset, 2);

        var packetTotalSize = BitConverter.ToInt16(header, offset);
        return packetTotalSize - EFBinaryRequestInfo.HEADER_SIZE;  //
    }

    protected override EFBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        throw new Exception();
    }
}

