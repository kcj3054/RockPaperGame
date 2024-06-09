
namespace Common;

public class PossibleGame
{
    public ushort RoomId { get; set; }
}

public class NewHost
{
    public ushort RoomId { get; set; }
}
public class Packet
{ 
    public ushort Size { get; set; }
    public ushort Id { get; set; }
}
public class VerifyIdRequest
{
    public string? NickName { get; set; }
}

public class ReadyRequest
{
    public int RoomId { get; set; }
    public string? ReadyState { get; set; }
    public string? NickName { get; set; }
}

public class NotifyReady
{
    public ushort RoomId { get; set; }
}

public class JoinGameRequest
{
    public string? NickName { get; set; }
}

public class JoinGame
{
    public CommonUserState? State { get; set; }
    public ushort RoomId { get; set; }

    public string UserName { get; set; }
}

//여러 유저들레디에 대한 결과.. 
public class ReadyResponse
{
    public bool ReadyResult { get; set; }
}

//EnterRoom의 ushort는 short는 이유는 -1 사용하기 위해서이다...
public class EnterRoomRequest
{
    public string? NickName { get; set; } = string.Empty;
    public short RoomNmber { get; set; }
}

public class EnterRoomResponse
{
    public int RoomNumber { get; set; }
    public CommonUserState UserState { get; set; }
}

public class EnterRoomToHostResponse
{ 
    public int RoomNumber { get; set; }
    public CommonUserState UserState { get; set; }
}

public class PossibleStartGame
{ 
    public ushort Count { get; set; }
    
    public  ushort RoomNumber { get; set; }
}
public class StartGameRequest
{
    public string? Result { get; set; } = string.Empty;
    public ushort UserCount { get; set; } 
    
    public ushort RoomNumber { get; set; } 
}

public class ExitRoomRequest
{
    public int RoomNumber { get; set; }
    public string Name { get; set; }
}
public class ExitPlayerRequest
{ 
    public string PlayerName { get; set; }
    public int RoomNumber { get; set; }
}
public class VerifyIdResponse
{ 
    public bool IsOk { get; set; }
}
public class AttackRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string Value { get; set; }  // 가위(1), 바위(2), 보(3) 중하나...
}

public class ShowRoomRequest
{
    public string? NickName { get; set; }
}

//값 복사가 일어나야하지 RoomManager를 이쪽에서 들고있으면 안된다 
public class ShowRoomResponse
{
    public List<Room> RooomInfo { get; set; } = new();
}

public class NotifyResultGame
{
    public ushort Result { get; set; }
}

public class NotifyResultGameToSpectator
{
    public Dictionary<string, ushort> UserResult { get; set; } = new();
}

