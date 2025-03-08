// using System.Net;
// using System.Net.Sockets;
// using Common;
//
// namespace Server;
//
// public class Connector
// {
//     private Func<Session> _sessionFactory;
//     
//     public void Connect(IPEndPoint endPoint, Func<Session>  sessionFactory)
//     {
//         Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//
//         SocketAsyncEventArgs args = new();
//         args.Completed += OnConnectCompleted;
//         args.RemoteEndPoint = endPoint;
//         args.UserToken = socket;
//
//
//         _sessionFactory = sessionFactory;
//     }
//
//     void RegisterConnect(SocketAsyncEventArgs args)
//     {
//         Socket socket = args.UserToken as Socket;
//
//         bool pending = socket.ConnectAsync(args);
//         if (pending == false)
//         {
//             OnConnectCompleted(null, args);
//         }
//     }
//
//     void OnConnectCompleted(object obj, SocketAsyncEventArgs args)
//     {
//         if (args.SocketError == SocketError.Success)
//         {
//             Session session = _sessionFactory.Invoke();
//             session.Init(args.ConnectSocket);
//            
//             // session.OnConnected(args.RemoteEndPoint);
//         }
//         else
//         {
//             Console.WriteLine($"OnConnected Fail :...xx");
//         }
//     }
// }