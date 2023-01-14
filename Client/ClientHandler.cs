using System.Text.Json;
using Client.Service;
using Common;
using static Common.RoomNumber;
using static Common.PacketId;
using static Common.RockPaper;
using static Common.Size;

namespace Client;

public class ClientHandler : Singleton<ClientHandler>
{
    private string _playerNickname = string.Empty;
    private int _roomNumber;
    private readonly Dictionary<int, Action<int, ArraySegment<byte>>> _packetHandlers = new();
    private bool _playerIdOk;
    private readonly ClientGameManager _clientGameManager = new();
    private bool isWait = false;
    
    public void RegisterPacketHandler()
    {
        _packetHandlers.Add((int)PacketId.EnterRoomResponse, OnPlayerEnterResponse);
        _packetHandlers.Add((int)NotifyGameStart, OnGameStart);
        _packetHandlers.Add((int)ExitPlayerResponse, OnPlayerExitRes); // 서버로부터 퇴장에대한 응답 패킷 
        _packetHandlers.Add((int)VerifyPlayerResponse, OnVerifyPlayerId);
        _packetHandlers.Add((int)NotifyGameRestart, OnGameReStart); // 게임 결과가 무승부일 경우 재시작 
        _packetHandlers.Add((int)NotifyGameResult, OnGameResult);
        _packetHandlers.Add((int)VictoryPlayerResponse, OnVictoryPlayer); // 상대방이 중도퇴실로인한 승리 
        _packetHandlers.Add((int)PacketId.ShowRoomResponse, OnShowRoomResponse);
        _packetHandlers.Add((int)NotifyAttackAgain, OnAttackAgain);
        _packetHandlers.Add((int)NotifyGameResultToSpectatorResponse, OnGameSpectate);
        _packetHandlers.Add((int)NotifyPossibleGameStart, OnPossibleGameStart); // todo : 수정 중 
        _packetHandlers.Add((int)EnterHostResponse, OnHostEnterResponse);
        _packetHandlers.Add((int)NotifyLooser, OnNotifyLooser); // 패배했다는 알림.. 
        _packetHandlers.Add((int)NotifyJoinGame, OnJoinGame); // 게임이 끝난 후 추후 게임 참가여부.. 
        _packetHandlers.Add((int)NotifyNotExist, OnNotExitRoomNumber);
        _packetHandlers.Add((int)NotifyExit, OnNotifyExit);
        _packetHandlers.Add((int)PacketId.NotifyReady, OnReadyNotify);
        _packetHandlers.Add((int)NotifyConvertState, OnConvertState);
        _packetHandlers.Add((int)NotifyNewhost, OnNotifyNewHost); ////
        _packetHandlers.Add((int)NotifyAskAgainPossibleGameStart, OnAskAgainPossibleGameStart);
        
        _packetHandlers.Add((int)NotifyWait, OnNotifyWait);
    }

    public void Execute(int headerInfo, ArraySegment<byte> bytes)
    {
        int id = headerInfo;
        if (_packetHandlers.ContainsKey(id))
        {
            try
            {
                _packetHandlers[id](headerInfo, bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void OnNotifyWait(int number, ArraySegment<byte> buffer)
    {
        if (!isWait)
        {
            Console.WriteLine("방자님이 대기를 결정하셨습니다.");
            isWait = true;
        }
    }
    
    private void OnAskAgainPossibleGameStart(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("게임 시작을 할 것인지 안할 것인지 제대로 다시 응답하시오");
        var possibleStartGame = JsonSerializer.Deserialize<PossibleStartGame>(buffer);
        var result = Console.ReadLine();

        ClientSession.Instance.socket?.Send(
            new MakePacket().StartGameReqPacket(result, possibleStartGame.Count, possibleStartGame.RoomNumber));
    }

    private void OnNotifyNewHost(int number, ArraySegment<byte> buffer)
    {
        var newHost = JsonSerializer.Deserialize<NewHost>(buffer);
        Console.WriteLine($"{newHost.RoomId}번 방의 새로운 방장이 되셨습니다.");
        ClientSession.Instance.Send(new MakePacket().PossibleGameRequestPacket(newHost.RoomId));
    }

    private void OnConvertState(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("이미 게임이 진행중입니다.. 현재 상태가 관전자로 변경되었습니다");
    }

    private void OnReadyNotify(int number, ArraySegment<byte> buffer)
    {
        var notifyReady = JsonSerializer.Deserialize<NotifyReady>(buffer);
        Console.WriteLine("제대로 다시 입력해주세요 레디 할 것 인가요???\nyes or no로만 대답하시오\n\n");
        var readyState = Console.ReadLine();
        ClientSession.Instance.Send(new MakePacket().ReadyReqPacket(readyState, _playerNickname, notifyReady.RoomId));
    }

    private void OnNotifyExit(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("레디 하지 않아서 퇴장당하셨습니다..다시 방을 고르시오..");
        ClientSession.Instance.Send(new MakePacket().ShowRoomReqPacket(_playerNickname));
    }

    private void OnNotExitRoomNumber(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("존재하지 않는 방 번호를 입력하셨습니다.. 다시 방리스트를 보여드릴게요..");
        ClientSession.Instance.Send(new MakePacket().ShowRoomReqPacket(_playerNickname));
    }

    // todo  : 기능 추가중..
    private void OnJoinGame(int number, ArraySegment<byte> buffer)
    {
        var joinGame = JsonSerializer.Deserialize<JoinGame>(buffer);
        Console.WriteLine("게임이 종료되었습니다 게임에 참가하시겠습니까? 참가하실 거면 yes를 퇴장하실 거면 no 입력");

        while (true)
        {
            var readyState = Console.ReadLine();
            //yes가 ready 패킷이다 
            if (readyState == "yes")
            {
                ClientSession.Instance.Send(new MakePacket().ReadyReqPacket(readyState, _playerNickname, joinGame.RoomId));
                return;
            }
            else if (readyState == "no")
            {
                PlayerExitRoom();
                return;
            }
            
            // switch (readyState)
            // {
            //     case "yes":
            //         
            //         break;
            //     case "no":
            //         PlayerExitRoom();
            //         break;
            //     // case "exit":
            //     //     ClientSession.Instance.Send(new MakePacket().ExitRoom(joinGame.RoomId, joinGame.UserName));
            //     //     break;
            // }
            Console.WriteLine("다시 입력해주세요");
        }
    }

    private void OnNotifyLooser(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("나는 패배했습니다..");
    }

    private void OnAttackAgain(int arg1, ArraySegment<byte> arg2)
    {
        Console.WriteLine("값을 다시 내라고 서버에서 응답왔습니다");
        _clientGameManager.Attack(_playerNickname);
    }

    private void OnHostEnterResponse(int number, ArraySegment<byte> buffer)
    {
        Console.Write("당신은 방장입니다 유저들이 레디를 할 때까지 잠시만 기다려주세요 \n\n");
    }

    private void OnPossibleGameStart(int number, ArraySegment<byte> buffer)
    {
        var possibleStartGame = JsonSerializer.Deserialize<PossibleStartGame>(buffer);
        Console.Write(
            $"방장님 게임을 시작하시겠습니까? 현재 레디를 한 플레이어들은 {possibleStartGame?.Count}명입니다  yes or no로 대답해주세요(대소문자 구분안해도됩니다~) : \n\n");

        var result = Console.ReadLine();

        ClientSession.Instance.socket?.Send(
            new MakePacket().StartGameReqPacket(result, possibleStartGame.Count, possibleStartGame.RoomNumber));
    }

    private void OnShowRoomResponse(int number, ArraySegment<byte> buffer)
    {
        ShowRoomResponse? roomsInfo = JsonSerializer.Deserialize<ShowRoomResponse>(buffer);
        foreach (var room in roomsInfo.RooomInfo)
        {
            if (room.IsRun)
                Console.WriteLine($"룸번호 {room.RoomId}번의 상태는 게임 진행 중이고 총 유저의 수는 {room.Count}입니다\n");
            else
                Console.WriteLine($"룸번호 {room.RoomId}번의 상태는 게임 진행 대기 중 총 유저의 수는 {room.Count}입니다\n");
        }

        if (roomsInfo.RooomInfo.Count == 0)
        {
            Console.WriteLine("현재 입장 가능한 방이 없습니다 새로운 방을 만들어야합니다 방의 번호는 게임서버 마음대로.\n\n");
            ClientSession.Instance.Send(new MakePacket().EnterRoomReqPacket(_playerNickname, (short)NEW_ROOM));
        }
        else
        {
            Console.WriteLine("몇번방으로 입장하실 건가요??? 새로운 방을 만들고 싶으시면 -1을 입력해주세요 \n");
            while (true)
            {
                var num = Console.ReadLine();
                if (short.TryParse(num, out short Num))
                {
                    ClientSession.Instance.Send(new MakePacket().EnterRoomReqPacket(_playerNickname, Num));
                    break;
                }

                Console.Write("제대로 입력해주세요");
            }
        }
    }

    private void OnGameReStart(int number, ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;

        Console.WriteLine("결과가 무승부라서 재시작하라네요 이번판은 이겨보자");
        var roomId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);

    }

    private void OnGameResult(int number, ArraySegment<byte> buffer)
    {
        var resultGame = JsonSerializer.Deserialize<NotifyResultGame>(buffer);

        switch (resultGame.Result)
        {
            case (int)WIN:
                Console.WriteLine("내가 승리했다.");
                break;
            case (int)DREW:
                Console.WriteLine("비겼습니다");
                break;
            case (int)LOOSE:
                Console.WriteLine("졌습니다");
                break;
        }
    }

    private void OnPlayerExitRes(int number, ArraySegment<byte> buffer)
    {
        //다른 에러나, 예외처리 생략 
        Console.WriteLine("성공적으로 퇴장하셨습니다. 잘가세요");
    }

    // 플레이어 아이디 검증 
    private void OnVerifyPlayerId(int number, ArraySegment<byte> buffer)
    {
        var verifyIdRequest = JsonSerializer.Deserialize<VerifyIdResponse>(buffer);

        if (verifyIdRequest != null && verifyIdRequest.IsOk)
        {
            _playerIdOk = true;
        }
        else
        {
            _playerIdOk = false;
        }
    }

    private void OnGameSpectate(int number, ArraySegment<byte> buffer)
    {
        var result = JsonSerializer.Deserialize<NotifyResultGameToSpectator>(buffer); // !!
        var userResult = result.UserResult;

        foreach (var user in userResult)
        {
            switch (user.Value)
            {
                case (ushort)WIN:
                    Console.WriteLine($"{user.Key}님이 승리하셨습니다");
                    break;
                case (ushort)DREW:
                    Console.WriteLine($"{user.Key}님의 결과는 무승부입니다");
                    break;
                case (ushort)LOOSE:
                    Console.WriteLine($"{user.Key}님의 결과는 패배입니다");
                    break;
            }
        }
    }

    private void OnGameStart(int number, ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;

        Console.WriteLine(" 현재 게임시작 최소인원이 모두 레디했습니다\n 게임을 시작합니다 \n\n");
        _clientGameManager.Attack(_playerNickname);
    }

    private void PlayerExitRoom()
    {
        Console.WriteLine($"나 방에서 나갈거야");
        ClientSession.Instance.Send(new MakePacket().ExitPlayerReqPacket(_playerNickname, _roomNumber));
    }

    //중도퇴실로인한 승리 
    private void OnVictoryPlayer(int number, ArraySegment<byte> buffer)
    {
        Console.WriteLine($" win..");
    }

    private void OnPlayerEnterResponse(int number, ArraySegment<byte> buffer)
    {
        var roomNumber = JsonSerializer.Deserialize<EnterRoomResponse>(buffer); // !!
        _roomNumber = roomNumber.RoomNumber;
        Console.WriteLine($"{_playerNickname}님이 룸번호 {_roomNumber}번방에 에 입장되었습니다\n\n");

        switch (roomNumber.UserState)
        {
            case CommonUserState.Spectator:
                //관전자일 경우.. 
                Console.WriteLine($"당신은 관전자입니다. 게임이 끝난다면 결과값을 받아 볼 수 있습니다");
                break;
            default:
                Console.WriteLine("게임을 레디 해도된다는 알림이 왔습니다 레디 할 것 인가요???\nyes or no로만 대답하시오\n\n");
                var readyState = Console.ReadLine();

                ClientSession.Instance.Send(new MakePacket().ReadyReqPacket(readyState, _playerNickname, _roomNumber));
                break;
        }
    }

    public Boolean GetPlayerIdOk()
    {
        return _playerIdOk;
    }

    public void SetPlayerNickname(string nickname)
    {
        _playerNickname = nickname;
    }

    public string GetPlayerNickname()
    {
        return _playerNickname;
    }
}