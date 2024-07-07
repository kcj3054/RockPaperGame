using System.Text.Json;
using Common;
using Server.Service;
using ServerCommon;
using static Common.RockPaper;
using static ServerCommon.UserState;

namespace Server;

// 네트워크 매니저가 다 붙기전에 Handler를 등록한다. 
// 작은 데이터 단위일 경우 List가 Dictionary보다 빠르기는하다. 

public partial class ServerHandler : Singleton<ServerHandler>
{
    private ServerSession? _serverSession = new();
    private readonly Dictionary<int, Action<int, ArraySegment<byte>>> _packetHandlers = new();
    private readonly bool _disconnected = false;
    private Object _lock = new();

    public void RegisterPacketHandler()
    {
        _packetHandlers.Add((int)PacketID.EnterRoomRequest, OnEnterRoomRequest);
        _packetHandlers.Add((int)PacketID.ReadyRequest, OnReadyRequest);
        _packetHandlers.Add((int)PacketID.ShowRoomRequest, OnShowRoomRequest);
        _packetHandlers.Add((int)PacketID.PlayerExitRequest, OnPlayerExitRequest); // client -> server 퇴장 요청 
        _packetHandlers.Add((int)PacketID.AttackRequest, OnPlayerAttackRequest); // 플레이어가 가위 바위 보 중하나를 냄
        _packetHandlers.Add((int)PacketID.VerifyPlayerRequest, OnVerifyPlayerId); //client -> server playerId 검증.. !
        _packetHandlers.Add((int)PacketID.StartGameRequest, OnStartGameRequest);
        _packetHandlers.Add((int)PacketID.PossibleGame, OnPossibleGame);
       // _packetHandlers.Add((int)PacketID.ExitRoom, OnExitRoom); todo : 추가할까 말까.. 
    }

    public void Execute(int headerInfo, ArraySegment<byte> bytes, ServerSession session)
    {
        var id = headerInfo;
        if (_packetHandlers.ContainsKey(id))
        {
            try
            {
                _serverSession = session;
                _packetHandlers[id](headerInfo, bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void ConvertUserState(User user, UserState userState = SPECTATOR)
    {
        user.UserState = userState;
    }

    private void MemberExit(string nickname, int roomId)
    {
        //해당 룸에서 
        var rooms = RoomManager.Instance.GameRooms;
        if (rooms.TryGetValue(roomId, out GameRoom gameRoom))
        {
            foreach (var member in gameRoom.Room)
            {
                if (member.NickName == nickname)
                {
                    gameRoom.Leave(member);
                    return;
                }
            }
        }
    }

    //레디를 몇명했는지 체크
    private ushort CheckReadyRoom(GameRoom room)
    {

        ushort cnt = 0;
        foreach (var state in room.Room)
        {
            if (state?.UserState == Ready || state?.UserState == HOST)
            {
                cnt++;
            }
        }

        //2명이성 레디를 했으니 게임 가능하다 
        if (cnt >= (int)UserCount.Min && (!room.IsRun))
        {
            NotifyPossibleGame(room, cnt);
        }
        return cnt;

    }

    //게임 시작해라 알리는 로직 
    private void NotifyStartGame(ushort roomNumber)
    {
        ;
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        var gameRooms = RoomManager.Instance.GameRooms;

        if (gameRooms.TryGetValue(roomNumber, out GameRoom gameRoom))
        {
            foreach (var member in gameRoom.Room)
            {
                if (sessionIdDic.ContainsKey(member.SessionId))
                {
                    switch (member.UserState)
                    {
                        //방장은 IN_GAME에 들어가도 방장이다 
                        case HOST:
                            sessionIdDic[member.SessionId]
                                .Send(new MakePacket().NotifyStartGamePacket());
                            member.UserState = HOST;
                            break;
                        case Ready:
                            sessionIdDic[member.SessionId]
                                .Send(new MakePacket().NotifyStartGamePacket());
                            member.UserState = IN_GAME;
                            break;
                        case SPECTATOR:
                            sessionIdDic[member.SessionId].Send(
                                new MakePacket().NotifySpectateGamePacket());
                            break;
                    }
                }
            }
            gameRoom.IsRun = true;
        }
    }

    private bool CheckRoomMemberValue(GameRoom gameRoom)
    {
        foreach (var member in gameRoom.Room)
        {
            if (member?.UserState != SPECTATOR && member?.AttackValue == (ushort)None)
            {
                return false;
            }
        }
        return true;
    }

    private void NotifyPossibleGame(GameRoom room, ushort cnt)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        foreach (var user in room.Room)
        {
            if (user?.UserState == HOST)
            {
                if (sessionIdDic.TryGetValue(user.SessionId, out Session? session))
                {
                    session.socket?.Send(new MakePacket().PossibleStartGamePacket(cnt, user.RoomId));
                    break;
                }
            }
        }
    }

    private void DecideGame(GameRoom room)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        // - 1. 가위 바위 보 3가지가 나오거나, 모두 다 동일한 값을 낸다면  다시 공격을 요청
        // - 2. 2가지가 된다면 지는 사람들을 빼자 user상태에서 isWin 조건..
        // - 3. 남은 애들끼리 다시 지지고 뽑고.. 
        // -모든 user의 isWin 상태가 true or false인지 

        //가위 바위 보 중 몇가지 종류인가?... 
        int rockPaperCount = RockPaperCount(room);

        //중도 퇴장했을 수도있다.. 체크.. 
        switch (rockPaperCount)
        {
            case var number when (number == 3) || (number == 1):
                //다시 내주세요~
                ReleaseAttackValue(room);
                //이 로직으로 흘러갈 것인지 한번 더 체크 
                NotifyAttackAgain(room);
                return;
            case 2:
                //판정 해당 룸에서 맴버들의 IsWin체크체크
                RemoveLooser(room);
                ReleaseAttackValue(room);
                //이 로직으로 흘러갈 것인지 한번 더 체크 
                //만약 결과값이 2이면서 남은 인원이 2명이상일 경우에만 다시 공격요청을.. 
                if (CalculateRoomPlayUserCount(room) >= (int)UserCount.Min)
                {
                    NotifyAttackAgain(room);
                    return;
                }
                break;
        }

        //최종 승리자 두둥!
        if (CalculateRoomPlayUserCount(room) == (int)UserCount.Alone)
        {
            foreach (var member in room.Room)
            {
                if (member?.IsWin == null)
                {
                    member.IsWin = true;
                    if (sessionIdDic.TryGetValue(member.SessionId, out Session? session))
                    {
                        // 최종승리자가 결정됨 
                        session.Send(new MakePacket().NotifyGameResultPacket((ushort)WIN));
                        break;
                    }
                }
            }
        }


        // 게임 끝나고 for문 한번 더 돌려야한다  위에서 같이 돌리면 부분적으로만된다 
        var userReuslt = new Dictionary<string, ushort>();

        foreach (var player in room.Room)
        {
            if (player?.UserState != SPECTATOR)
            {
                if (player.IsWin == true)
                    userReuslt.TryAdd(player.NickName, (ushort)WIN);
                else
                    userReuslt.TryAdd(player.NickName, (ushort)LOOSE);
            }
        }
        // 게임 끝..  관전자들에게는 게임에 참가할 것인지 물어보고
        //      일반유저들에게는 게임을 한번 더 할 것인지 물어보기 .. 
        foreach (var player in room.Room)
        {
            if (player?.UserState == SPECTATOR)
            {
                sessionIdDic[player.SessionId].Send(new MakePacket().NotifyGameResultToSpectatorPacket(userReuslt));
            }
            room.IsRun = false;
        }

        //정상적인 게임 종료시.. 
        RemoveUserState(room);
        JoinGame(room);
    }

    //게임이 끝난 후 userState를 풀어버리기 .. 
    private void RemoveUserState(GameRoom room)
    {
        foreach (var member in room.Room)
        {
            if (member?.UserState != HOST)
                member.UserState = PLAYER;

            member.AttackValue = (ushort)None;
            member.IsWin = null;
        }
    }

    //방장에게도 게임에 참가할 것인지는 물어봐야한다 
    private void JoinGame(GameRoom room)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();

        foreach (var member in room.Room)
        {
            if (member.UserState != HOST)
            {
                var 

                sessionIdDic.TryGetValue(member.SessionId, out Session? serverSession);
                serverSession?.Send(new MakePacket().JoinGamePacket(CommonUserState.Player, member.RoomId, member.NickName));
            }
            else if (member.UserState == HOST)
            {
                //todo : 체크 ..!
                sessionIdDic.TryGetValue(member.SessionId, out Session? serverSession);
                serverSession?.Send(new MakePacket().JoinGamePacket(CommonUserState.Host, member.RoomId, member.NickName));
            }
        }
    }


    //가위 바위 보 중 2가지를 냈을 경우 진 사람들 배제
    // - 가위 바위
    // - 가위  보
    // - 바위  보
    private void RemoveLooser(GameRoom room)
    {
        Dictionary<RockPaper, User> dictionary = new();

        foreach (var member in room.Room)
        {
            switch (member?.AttackValue)
            {
                case (ushort)SCISSORS:
                    dictionary.TryAdd(SCISSORS, member);
                    break;
                case (ushort)ROCK:
                    dictionary.TryAdd(ROCK, member);
                    break;
                case (ushort)PAPER:
                    dictionary.TryAdd(PAPER, member);
                    break;
            }

        }

        if (dictionary.ContainsKey(SCISSORS) && dictionary.ContainsKey(ROCK))
        {
            ScissorVersusRock(room);
        }
        else if (dictionary.ContainsKey(SCISSORS) && dictionary.ContainsKey(PAPER))
        {
            ScissorVersusPaper(room);
        }
        else if (dictionary.ContainsKey(ROCK) && dictionary.ContainsKey(PAPER))
        {
            RockVersusPaper(room);
        }
    }

    private void ScissorVersusRock(GameRoom room)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        foreach (var member in room.Room)
        {
            if (member?.AttackValue == (ushort)SCISSORS && member.IsWin == null)
            {
                member.IsWin = false;

                if (sessionIdDic.ContainsKey(member.SessionId))
                {
                    sessionIdDic.TryGetValue(member.SessionId, out Session? session);
                    session?.Send(new MakePacket().NotifyLooserPacket());
                }
            }
        }
    }

    private void ScissorVersusPaper(GameRoom room)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        foreach (var member in room.Room)
        {
            if (member?.AttackValue == (ushort)PAPER && member.IsWin == null)
            {
                member.IsWin = false;
                if (sessionIdDic.TryGetValue(member.SessionId, out Session? session))
                {
                    session?.Send(new MakePacket().NotifyLooserPacket());
                }
            }
        }
    }

    private void RockVersusPaper(GameRoom room)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        foreach (var member in room.Room)
        {
            if (member?.AttackValue == (ushort)ROCK && member.IsWin == null)
            {
                member.IsWin = false;
                if (sessionIdDic.TryGetValue(member.SessionId, out Session? session))
                {
                    session.Send(new MakePacket().NotifyLooserPacket());
                }
            }
        }
    }

    private void ReleaseAttackValue(GameRoom room)
    {
        foreach (var member in room.Room)
        {
            if (member?.UserState != SPECTATOR && member?.IsWin == null)
            {
                member.AttackValue = (ushort)None;
            }
        }
    }

    ////승패가 정해지지않은 플레이어분들에게 다시 요청.. 
    private void NotifyAttackAgain(GameRoom room)
    {
        var sessions = SessionManager.Instance.GetSessionIdDic();
        foreach (var member in room.Room)
        {
            if (member?.IsWin == null && member?.UserState != SPECTATOR)
            {
                if (sessions.ContainsKey(member.SessionId))
                {
                    sessions[member.SessionId].Send(new MakePacket().NoifyAttackAgainPacket());
                }
            }
        }
    }

    //아직 게임할 사람들..
    private ushort CalculateRoomPlayUserCount(GameRoom room)
    {
        ushort cnt = 0;
        foreach (var member in room.Room)
        {
            if (member?.UserState != SPECTATOR && member?.IsWin == null)
            {
                cnt++;
            }
        }
        return cnt;
    }

    // 가위 바위 보 종류 수 
    private int RockPaperCount(GameRoom room)
    {
        Dictionary<RockPaper, ushort> count = new();
        foreach (var member in room.Room)
        {
            if (member?.UserState != SPECTATOR && member.IsWin == null)
            {
                count.TryAdd((RockPaper)member.AttackValue, 1);
            }
        }
        return count.Count;
    }

    private void ServerSend(ArraySegment<byte> segment)
    {
        if (_serverSession != null)
        {
            _serverSession.Send(segment);
        }
        else
        {
            Console.WriteLine($"현재 session이 null입니다");
        }
    }
}