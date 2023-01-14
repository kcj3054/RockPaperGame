using System.Text.Json;
using Common;

namespace Server;

public partial class ServerHandler
{
    private void OnShowRoomRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var showRoomRequest = JsonSerializer.Deserialize<ShowRoomRequest>(buffer);
        if (showRoomRequest != null)
            Console.WriteLine($"{showRoomRequest.NickName}님이 룸 정보를 요청했습니다 ");
        
        

        lock (_lock)
        {
            List<Room> rooms = new();
            var gameRooms = RoomManager.Instance.GameRooms;
            foreach (var gameRoom in gameRooms)
            {
                rooms.Add(
                    new Room()
                    {
                        RoomId = gameRoom.Value.RoomId, IsRun = gameRoom.Value.IsRun, Count = gameRoom.Value.Room.Count
                    });
            }
            ServerSend(new MakePacket().ShowRoomResPacket(rooms));
        }
    }
}