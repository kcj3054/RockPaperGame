namespace Common
{
    public enum PacketID
    {
        // �� ���� ��û �� ���� ��Ŷ (100-199)
        EnterRoomRequest = 100,
        EnterRoomResponse = 101,
        EnterHostResponse = 102,
        ExitRoom = 103,

        // �÷��̾� ���� ��û �� ���� ��Ŷ (200-299)
        PlayerExitRequest = 200,
        VerifyPlayerRequest = 201,
        VerifyPlayerResponse = 202,
        VictoryPlayerResponse = 203,

        // ���� ���� �˸� �� ��û ��Ŷ (300-399)
        NotifyGameStart = 300,
        NotifyGameSpectate = 301,
        NotifyGameResultToSpectatorResponse = 302,
        NotifyGameResult = 303,
        NotifyGameRestart = 304,
        StartGameRequest = 305,

        // �غ� ���� �˸� �� ��û ��Ŷ (400-499)
        NotifyReady = 400,
        ReadyResponse = 401,
        ReadyRequest = 402,

        // �� ǥ�� ���� ��û �� ���� ��Ŷ (500-599)
        ShowRoomRequest = 500,
        ShowRoomResponse = 501,

        // ���� ���� ��û �� �˸� ��Ŷ (600-699)
        AttackRequest = 600,
        NotifyAttackAgain = 601,

        // ���� ���� �˸� ��Ŷ (700-799)
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

        // ��� ��� ��Ŷ (900-999)
        ResultRequest = 900
    }
}
