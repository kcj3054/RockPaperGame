using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Common;

namespace Server;

public class Listener
{
    private Socket? _listenSocket;
    private Action<Socket>? _onAcceptHandler;
    
    public void Init(EndPoint endPoint, Action<Socket> acceptHandler)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10);

        _onAcceptHandler += acceptHandler;

        SocketAsyncEventArgs acceptArgs = new();
        acceptArgs.Completed += (OnAcceptCompleted);
        
        RegisterAccept(acceptArgs);
    }

    void RegisterAccept(SocketAsyncEventArgs args)
    {
        // 이벤트 재사용시 클리어
        args.AcceptSocket = null; 
        
        if(_listenSocket == null)
            return;
        
        bool pending = _listenSocket.AcceptAsync(args);
        if (pending == false)
        {
            OnAcceptCompleted(new object(), args);
        }
    }
    void OnAcceptCompleted(Object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success && _onAcceptHandler != null && args.AcceptSocket != null)
        {
            _onAcceptHandler.Invoke(args.AcceptSocket);
        }
        
        #if DEBUG
        else
        {
            Console.WriteLine("OnAcceptCompleted Error");
        }
        #endif
        
        RegisterAccept(args);
    }
    
}