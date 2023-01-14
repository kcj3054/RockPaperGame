using System.Net.Sockets;
using System.Runtime.InteropServices;
using Server;
using Server.Service;
namespace Common;
public class Session
{
    public int sessionId;
    private static readonly int HeaderSize = 4;
    public Socket? socket;
    private GameManager _gameManager = new();
    private int _disconnected;
    protected object _lock = new();
    
    // public int cnt = 1;  //테스트 용도. 
    public void Init(Socket socket, int id)
    {
        this.socket = socket;
        sessionId = id;

        if (this.socket.Connected)
        {
#if DEBUG
            Console.WriteLine("Session클래스의 _socket이 연결되었습니다 ");
# endif

            Console.WriteLine(" 유저가 접속했습니다. ");
            SocketAsyncEventArgs? recvArgs = new();
            SocketAsyncEventArgs? sendArgs = new();

            sendArgs.Completed += (OnSendCompleted);
            recvArgs.Completed += (OnRecvCompleted);

            recvArgs.SetBuffer(new byte[2048], 0, 2048);
            RegisterRecv(recvArgs);

           // Register();
        }
        else
        {
            Console.WriteLine("유저가 연결되어있지않습니다 ");
        }
    }
    private void RegisterRecv(SocketAsyncEventArgs args)
    {
        if (socket == null || (!socket.Connected))
            return;

        bool pending = socket.ReceiveAsync(args);
        if (pending == false)
        {
            OnRecvCompleted(null, args);
        }
    }
    private void OnRecvCompleted(Object? sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
            RegisterRecv(args);
        }
        else
        {
            //SessionManager.Instance.SetMustLoose(sessionId, true); // 여기는 해당 sessionId에 무조건 지는 것을 넣은 것..
            var (tmpSessionId, tmpSession) = SessionManager.Instance.DisappearUser(sessionId);
            
            //todo : 수정 예정 . 핑퐁 한다면 수정 
            Thread.Sleep(1000);
            _gameManager.MustWinPlayer(sessionId);
            SessionManager.Instance.ClearSession(tmpSessionId, tmpSession);
        }
    }
    //중도 연결이 끊어졌을 대오류가 발생한다.. 
    public void Send(ArraySegment<byte> buffer)
    {
        if (socket != null && socket.Connected)
        {
            socket.Send(buffer);
        }
    }
    private void OnRecv(ArraySegment<byte> buffer)
    {
        int sourceIndex = 0;
        while (true)
        {
            // Console.WriteLine($"OnRecv : {buffer.Count}");
            if (buffer.Count < HeaderSize)
                break;
    
            //사이즈를 넣을때 json + hear사이즈로 넣자 
            var size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            var id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            //Console.WriteLine($"id : {id}");
            if (buffer.Count < size)
                break;
            
            // 여기서 . 헤더크가 보고, 패킷아이디에 따른 바디가 제대로왔는지보면서. 
            var verifySegment = new ArraySegment<byte>(new byte[size]);
    
            //Array.Copy에 sourceIndex 0 수정하자
            Array.Copy(buffer.Array, sourceIndex, verifySegment.Array, verifySegment.Offset, size);
            var id222 = BitConverter.ToUInt16(verifySegment.Array, verifySegment.Offset + 2);
            OnRecvPacket(verifySegment.Array);  // verifySegment가 아니라 verifySegment.Array를 보내보자  
           
            // 패킷아이디 410짜리가 두번 와있어서 2번 분리한다... 
            //Console.WriteLine($"분리전 : {buffer.Count}, 패킷  아이디 {id222}");
            buffer = buffer.Slice(size);
            //Console.WriteLine($"분리후 : {buffer.Count}");
            
            sourceIndex += size; // 핵심.! 
        }
    }
    protected virtual void OnRecvPacket(ArraySegment<byte> buffer)
    {
        
    }

    private void OnSendCompleted(Object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success && args.Buffer != null)
        {
            socket?.Send(args.Buffer);
        }
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        socket?.Shutdown(SocketShutdown.Both);
        socket?.Close();
    }
}