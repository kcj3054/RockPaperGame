using System.Collections.Concurrent;
using Common;
namespace Server;

public class RoomManager : Singleton<RoomManager>
{
    public ConcurrentDictionary<int, GameRoom> GameRooms { get; set; } = new(); //roomId
}