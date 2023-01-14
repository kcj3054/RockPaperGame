using System.Net;
using System.Net.Sockets;
using Common;
namespace Client;

public class ClientSession : Session
{
    static ClientSession clientSession = new();
    public static ClientSession Instance { get { return clientSession; } }
    
    protected sealed override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        if(buffer.Array == null)
            return;
        
        //사용했던 패킷이 재활용되고있다...?...
        lock (_lock)
        {
            var id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            var size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            
            //Console.WriteLine($"ClientSession packet Id : {id}");
            //Console.WriteLine($"ClientSession packet Size : {size}");
            
            ClientHandler.Instance.Execute(id, buffer.Slice((int)Size.HeaderSize));
        }
    }
    
    public Socket? OnConnect(EndPoint endPoint)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(endPoint);
            return socket;
        }
        catch 
        {
            Console.WriteLine("아직 서버가 일어나지 않았습니다 한번 더 시도하겠습니다");
        }
        return null;
    }
    
    public bool OnConnected()
    {
        if (socket != null && socket.Connected)
        {
            return true;
        }
        return false;
    }
}