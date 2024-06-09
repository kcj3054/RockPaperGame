namespace Common
{
    public enum PacketID
    {
        // 방 관련 요청 및 응답 패킷 (100-199)
        EnterRoomRequest = 100,
        EnterRoomResponse = 101,
        EnterHostResponse = 102,
        ExitRoom = 103,

        // 플레이어 관련 요청 및 응답 패킷 (200-299)
        PlayerExitRequest = 200,
        VerifyPlayerRequest = 201,
        VerifyPlayerResponse = 202,
        VictoryPlayerResponse = 203,

        // 게임 관련 알림 및 요청 패킷 (300-399)
        NotifyGameStart = 300,
        NotifyGameSpectate = 301,
        NotifyGameResultToSpectatorResponse = 302,
        NotifyGameResult = 303,
        NotifyGameRestart = 304,
        StartGameRequest = 305,

        // 준비 관련 알림 및 요청 패킷 (400-499)
        NotifyReady = 400,
        ReadyResponse = 401,
        ReadyRequest = 402,

        // 방 표시 관련 요청 및 응답 패킷 (500-599)
        ShowRoomRequest = 500,
        ShowRoomResponse = 501,

        // 공격 관련 요청 및 알림 패킷 (600-699)
        AttackRequest = 600,
        NotifyAttackAgain = 601,

        // 게임 상태 알림 패킷 (700-799)
        NotifyPossibleGameStart = 700,
        NotifyAskAgainPossibleGameStart = 701,
        NotifyWait = 702,
        NotifyLooser = 703,
        NotifyJoinGame = 704,
        JoinGameRequest = 705,
        JoinGameToHost = 706,
        NotifyExit = 707,
        NotifyNotExist = 708,
        NotifyConvertState = 709,
        NotifyNewHost = 710,
        PossibleGame = 711,

        // 경기 결과 패킷 (900-999)
        ResultRequest = 900
    }
}
