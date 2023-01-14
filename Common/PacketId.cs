namespace Common;

public enum PacketId
{
    EnterRoomRequest = 1,
    PlayerExitRequest = 3,
    ResultRequest = 5,
    VerifyPlayerRequest = 21,
    
    NotifyReady = 222,
    ReadyResponse = 223,
    ReadyRequest = 226,
    
    EnterRoomResponse = 2,
    EnterHostResponse = 301,
    
    ExitPlayerResponse = 4,
    NotifyGameStart = 6,
    NotifyGameSpectate = 7,
    
    NotifyGameResultToSpectatorResponse = 100,
    
    NotifyGameResult = 101,
    
    VerifyPlayerResponse = 500,
    VictoryPlayerResponse  = 511,
    
    NotifyGameRestart = 102,
    
    ShowRoomRequest = 200,
    ShowRoomResponse = 201,
    
    AttackRequest = 202,

    NotifyAttackAgain = 241,
    
    NotifyPossibleGameStart = 300,
    NotifyAskAgainPossibleGameStart = 333,
    NotifyWait = 334,
    StartGameRequest =400,
    
    NotifyLooser = 401,
    NotifyJoinGame = 402,
    JoinGameRequest = 403,
    JOIN_GAME_TO_HOST = 404,
    NotifyExit = 405,
    
    NotifyNotExist = 407,
    NotifyConvertState = 408,
    NotifyNewhost = 410,
    PossibleGame = 411,
    
    ExitRoom = 500,
}


