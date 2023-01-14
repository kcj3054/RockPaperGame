using System.Net;
using System.Net.Sockets;
using Server;


ServerHandler.Instance.RegisterPacketHandler();

Listener listener = new();
IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), 11111);
listener.Init(endPoint, OnAcceptHandler);

var line = Console.ReadLine();

void OnAcceptHandler(Socket clientSocket)
{
    if (clientSocket.Connected)
    {
       SessionManager.Instance.Generate(clientSocket);  // 접속한 session을 sessionManager의 Dic에 넣어놓기 ! 
    }
}