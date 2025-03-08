namespace ServerCommon;
public enum UserState
{
    PLAYER = 5, // 방에 입장했을 때 상태 
    SPECTATOR = 6, // 관전자 
    HOST = 7, // 방에 입장했을 때 방장인 플레이어의 상태 
    Ready = 8, // 준비는 준비를 했다는 패킷용도로 쓰자 
    IN_GAME = 9, // 게임 플레이 중인 유저 
    Eixt = 10, // 퇴장 유저 
}
public enum Size
{
    HEADER_SIZE = 4,
}

public enum UserCount
{
    Min = 2,
    Alone = 1,
}

public enum Name
{
    MaxLength = 10,
}
