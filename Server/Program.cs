using System.Net;
using System.Net.Sockets;
using Server;

/*
 * packet
 * header -> packetid, packe size
 * body => 내용물 data
 */

/*
 *  패킷 직렬화 -> serialize. 
 *  xml
 *  packet serialize, deserialize.  
 * protobuf  ->  packet1 {int long }  // 압축 !, c# c++
 * flatbuf -> c++, zero copy.. protobuf 성능 flatbuf
 *  ->>> memory   
 * messagePack ?!
 */

ServerHandler.Instance.RegisterPacketHandler();

Listener listener = new();
IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), 7700);
listener.Init(endPoint, OnAcceptHandler);

Console.ReadLine();

while (true)
{
    ;
}

void OnAcceptHandler(Socket clientSocket)
{
    if (clientSocket.Connected)
    {
       SessionManager.Instance.Generate(clientSocket);  // 접속한 session을 sessionManager의 Dic에 넣어놓기 ! 
    }
}