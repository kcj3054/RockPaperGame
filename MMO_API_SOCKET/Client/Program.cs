using System.Net;
using System.Net.Sockets;
using Client;

ClientHandler.Instance.RegisterPacketHandler();  
IPEndPoint edPoint = new (IPAddress.Parse("127.0.0.1"), 123);

while (true)
{
    Console.WriteLine("게임 서버에 입장 중입니다 !");
    Socket? socket  = ClientSession.Instance.OnConnect(edPoint);

    if (socket != null)
    {
        if (ClientSession.Instance.OnConnected())
        {
            Console.WriteLine("게임 연결이 완료되었습니다.");
            ClientSession.Instance.Init(socket, 0);
            break;
        }
    }
    else
    {
        Console.WriteLine("연결하지 못했습니다. 다시 시도합니다. ");
    }
}

StartGame startGame = new();
startGame.Start();