using System.Text.Json;
using Common;

namespace Server;

public partial class ServerHandler
{
    private void OnPossibleGame(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var possibleGame = JsonSerializer.Deserialize<PossibleGame>(buffer);
        if(possibleGame == null)
            return;
        
        var gameRooms = RoomManager.Instance.GameRooms;
        if (gameRooms.TryGetValue(possibleGame.RoomId, out GameRoom? room))
        {
            CheckReadyRoom(room);
        }
    }
}