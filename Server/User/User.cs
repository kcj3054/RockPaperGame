using ServerCommon;

namespace Server;

public class User
{
    public UserState UserState { get; set; }
    //게임 결과 체크..
    public bool? IsWin { get; set; }
    public ushort AttackValue { get; set; }
    public string NickName { get; set; } = string.Empty;
    public ushort RoomId { get; set; }
    public int SessionId { get; set; }
    public ulong Order { get; set; }
}



