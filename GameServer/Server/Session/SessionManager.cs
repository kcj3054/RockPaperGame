using System.Collections.Concurrent;
using System.Net.Sockets;
using Common;
using ServerCommon;

namespace Server;

public class SessionManager : Singleton<SessionManager>
{
    private int _sessionId;
    private Object _lock = new();

    private readonly ConcurrentDictionary<int, Session> _sessionsDic = new();
    private readonly ConcurrentDictionary<Session, User> _userDic = new(); // user자체를 관리하는 Dictionary, sessionId도 있당
    private readonly ConcurrentDictionary<string, bool> _nicknameDic = new();

    public ConcurrentDictionary<string, bool> GetNicknameDic()
    {
        return _nicknameDic;
    }

    public ConcurrentDictionary<int, Session> GetSessionIdDic()
    {
        return _sessionsDic;
    }

    public ConcurrentDictionary<Session, User> GetUserDic()
    {
        lock (_lock)
        {
            return _userDic;
        }
    }

    public void Generate(Socket socket)
    {
        lock (_lock)
        {
            _sessionId++;
            ServerSession serverSession = new();
            serverSession.sessionId = _sessionId;
            serverSession.Init(socket, _sessionId);

            _sessionsDic.TryAdd(_sessionId, serverSession);
        }
        Console.WriteLine($"Connected : sessionId{_sessionId}이 생성되었습니다.");
    }

    public (int, Session?) DisappearUser(int sessionId)
    {

        int tmpSessionId = sessionId;
        var gameRooms = RoomManager.Instance.GameRooms;

        if (_sessionsDic.TryGetValue(tmpSessionId, out Session? tmpSession))
        {
            //user를 만들기 전 일 수도있다 
            if (_userDic.TryGetValue(tmpSession, out User? user))
            {
                if (gameRooms.TryGetValue(user.RoomId, out GameRoom? room))
                {
                    if (user.UserState == UserState.HOST)
                    {
                        NewHost(room);
                    }
                    else if (user.UserState != UserState.HOST)
                    {
                        //순번표는 반납해야한다
                        RemoveOrderNumber(user, room);
                    }

                    if (_nicknameDic.TryRemove(user.NickName, out _))
                    {
                        LeveRoom(room, user);
                    }
                }
                return (tmpSessionId, tmpSession);
                //session정리는 추후에 
            }
            return (tmpSessionId, null);
        }
        return (0, null);
    }

    private void LeveRoom(GameRoom room, User user)
    {
        room.Leave(user);
        Console.WriteLine($"{user.NickName}님이 {room.RoomId}에서 퇴장하셨습니다");
        if (room.Room.Count == 0)
            room.IsRun = false;
    }

    private void RemoveOrderNumber(User user, GameRoom room)
    {
        var userOrder = room.GetUserOrder();
        List<Tuple<User, ulong>> lists = new();

        while (userOrder.Count != 0)
        {
            var player = userOrder.Dequeue();
            lists.Add(new Tuple<User, ulong>(player, player.Order));
        }

        if (lists.Remove(new Tuple<User, ulong>(user, user.Order)))
        {
            Console.WriteLine($"{user.NickName}님의 순번표 {user.Order}번을 지웠습니다");
            foreach (var list in lists)
            {
                userOrder.Enqueue(list.Item1, list.Item2);
            }
        }
    }

    //playerState가 ready인지 , ready라면 풀기 
    private bool PlayerStateCheck(User user)
    {
        if (user.UserState == UserState.Ready)
        {
            user.UserState = UserState.PLAYER;
            //user 상태변경을 알리는 패킷 쏘기 
            return true;
        }
        return false;
    }

    private void NewHost(GameRoom room)
    {
        var userOrder = room.GetUserOrder();
        if (userOrder.Count >= 2)
        {
            userOrder.Dequeue();
            var peek = userOrder.Peek();
            peek.UserState = UserState.HOST;
            Console.WriteLine($"{peek.NickName}님이 새로운 방장이 되셨습니다");

            if (_sessionsDic.ContainsKey(peek.SessionId))
            {
                Console.WriteLine($"NotifyNewhost send!!");
                _sessionsDic[peek.SessionId].Send(new MakePacket().NewHostPacket(peek.RoomId));
                var test1 = new MakePacket().NewHostPacket(peek.RoomId);
                Console.WriteLine($" 사이즈 : {test1.Count}");
            }
        }
    }

    public void ClearSession(int sessionId, Session? session)
    {
        if (_sessionsDic.TryRemove(sessionId, out _))
        {
            Console.WriteLine($"{sessionId}번 session이 끊어졌습니다");
            if (session != null && _userDic.TryRemove(session, out _))
            {
                Console.WriteLine($"{sessionId}가 가지고 있는 user가 사라졌습니다.");
                session.Disconnect();
            }
        }
    }

}