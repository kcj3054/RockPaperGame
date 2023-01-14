using System.Runtime.CompilerServices;
using Common;
using Server;
using ServerCommon;
using static Common.RoomNumber;
using static ServerCommon.UserState;
using static Common.RockPaper;

namespace Server.Service;

public class GameManager : Singleton<GameManager>
{
//진행 중인 방에 입장했을 때는 spectator가 되어야한다 
    public (int, UserState) EnterRoom(ServerSession serverSession, int roomNumber)
    {
        var userDic = SessionManager.Instance.GetUserDic();
        User user = new();

        //새로운 방을 만들자 새로운 방을 만들 때 증설할 수도 있고, 진짜 방이 하나도 없으면 1번방 만들어주면된다 
        var rooms = RoomManager.Instance.GameRooms; // todo : 수정중... 
        if (roomNumber == (short)NEW_ROOM)
        {
            if (rooms.Count == 0)
            {
                roomNumber = 1;
            }
            else
            {
                //문제가 발생할 수 있으나 작은 값들만 사용할 것이다 그래도 추후에 변경
                roomNumber = rooms.Count;
                Interlocked.Increment(ref roomNumber);
            }

            //todo : check 한 클라유저들마다 자신의 user 개체를 건드린다
            if (userDic.TryGetValue(serverSession, out user))
            {
                var gameRoom = new GameRoom();

                gameRoom.RoomId = (ushort)roomNumber;  // todo : 질문.RoomId는 공유영역이기는 하지만.. 위에서 interlocked.increment로 증가시켰다.. 
                gameRoom.Enter(user);
                gameRoom.SetUserOrder(user);

                user.UserState = HOST;
                user.RoomId = gameRoom.RoomId;

                RoomManager.Instance.GameRooms.TryAdd(gameRoom.RoomId, gameRoom);
                Console.WriteLine($"EnterROOM PlayerId : {user.NickName}");
                Console.WriteLine($"{user.NickName}님이 {gameRoom.RoomId}번 방에 {user.UserState}상태로 입장하셨습니다");
            }
        }
        else
        {
            var gameRooms = RoomManager.Instance.GameRooms;
            //현재 플레이어가 입장하고싶어하는 룸 , 해당 룸의 인원이 0명이면 처음으로 1인 된 유저는 자동으로 방장이된다 
            if (gameRooms.TryGetValue((ushort)roomNumber, out GameRoom room))
            {
                if (userDic.TryGetValue(serverSession, out user))
                {
                    if (room.IsRun && userDic.ContainsKey(serverSession))
                    {
                        user.UserState = SPECTATOR;
                    }
                    else if (!room.IsRun && userDic.ContainsKey(serverSession) && room.Room.Count == 0)
                    {
                        user.UserState = HOST;
                    }
                    else if (!room.IsRun && userDic.ContainsKey(serverSession) && room.Room.Count != 0)
                    {
                        user.UserState = PLAYER;
                    }
                    room.Enter(user);
                    room.SetUserOrder(user);

                    user.RoomId = (ushort)roomNumber; // todo : check, 락  필요없을 듯 ? 
                    Console.WriteLine($"{user.NickName}님이 {room.RoomId}번 방에 {user.UserState}상태로 입장하셨습니다");
                }
            }
        }
        return (roomNumber, user.UserState);
    }

    //해당 룸에 중도 끊어진 것이 있는지 체크하는 함수 
    // 무조건 이기는 플레이어가 존재하는지, 그렇다면 해당 플레이어 아이디를 return .
    public void MustWinPlayer(int sessionId)
    {
        User? winPlayer = new();
        GameRoom? gameRoom = new();

        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        var playerIdDic = SessionManager.Instance.GetUserDic();

        //todo : 수정! 
        if (sessionIdDic.TryGetValue(sessionId, out Session? session))
        {
            if (playerIdDic.TryGetValue(session, out User? loosePlayer))
            {
                var gameRooms = RoomManager.Instance.GameRooms;

                // var gameRooms = RoomManager.Instance.gameRooms;
                if (gameRooms.TryGetValue(loosePlayer.RoomId, out gameRoom))
                {
                    //게임이 진행중이지 않는 경우이거나 결과가 정해지지않은 유저가 2명이상인 경우 불필요. 
                    if (!gameRoom.IsRun || InGamePlayNumber(gameRoom) >= 2)
                        return ;
                    
                    foreach (var user in gameRoom.Room)
                    {
                        if (user?.NickName != loosePlayer.NickName && user.IsWin == null && user.UserState != SPECTATOR)
                        {
                            winPlayer = user;
                            gameRoom.IsRun = false;
                            break;
                        }
                    }
                }
            }
        }
        Dictionary<string, ushort> userResults = new();
        
        if (sessionIdDic.TryGetValue(winPlayer.SessionId, out session))
        {
            userResults.TryAdd(winPlayer.NickName, (ushort)WIN);
            session.Send(new MakePacket().VictoryPlayerPacket()); 
            var t1 = new MakePacket().VictoryPlayerPacket();

            foreach (var user in gameRoom.Room)
            {
                if (user.UserState == SPECTATOR)
                {
                    if (sessionIdDic.TryGetValue(user.SessionId, out session))
                        session.Send(new MakePacket().NotifyGameResultToSpectatorPacket(userResults));
                }
                else
                {
                    if (sessionIdDic.TryGetValue(user.SessionId, out session))
                    {
                        session.Send(new MakePacket().JoinGamePacket(CommonUserState.Player, gameRoom.RoomId, user.NickName));
                    }
                          
                }
            }
            ClearMember(gameRoom);
        }
    }

    private void ClearMember(GameRoom gameRoom)
    {
        foreach (var member in gameRoom.Room)
        {
            member.AttackValue = (ushort)None;
            member.IsWin = null;
            if (member.UserState != HOST)
                member.UserState = PLAYER;
        }
    }

   
    private int InGamePlayNumber(GameRoom room)
    {
        int cnt = 0;
        foreach (var member in room.Room)
        {
            if (member?.UserState != SPECTATOR && member?.IsWin == null)
            {
                Interlocked.Increment(ref cnt);
            }
                    
        }
        return cnt;
    }
}