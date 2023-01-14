namespace Server;

//게임룸에 입장한 유저들을 관리할 때 nickname이나 session으로 관리하는 것이 아닌 User개체 자체로 관리하자 

public class GameRoom
{
    private static ulong _orderNumber = 1;
    private readonly Object _lock = new();
    private readonly PriorityQueue<User, ulong> _userOrder = new(); // 방에 들어온 가장 최근 사람을 방장으로 만들기 위해.. 
    public ushort RoomId { get; set; }

    //게임이 진행중인지 체크를 해야지 serverHandler쪽에서 게임 중인방에게는 공격요청을 안할 수 있다 
    public bool IsRun { get; set; }
    public List<User?> Room { get; private set; } = new();

    //사용하는 곳 수정... 
    public PriorityQueue<User, ulong> GetUserOrder()
    {
        return _userOrder;
    }

    public void SetUserOrder(User user)
    {
        lock (_lock)
        {
            _userOrder.Enqueue(user, user.Order);
        }
    }

    public void Enter(User? user)
    {
        Room.Add(user);
      
        Interlocked.Increment(ref _orderNumber);
        //todo check .
        user.Order = _orderNumber;
    }

    //Leve할 때 Room에서만 제거하고, _userOrder는 빼지말자. 
    public void Leave(User user)
    {
        if (Room.Remove(user))
        {
            Console.WriteLine($"{user.NickName}님이 {RoomId}번방에서 퇴장하셨습니다 ");
        }
    }
}