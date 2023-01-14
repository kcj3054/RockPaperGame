using System.Text.Json;
using Common;
using ServerCommon;

namespace Server;

public partial class ServerHandler
{ 
    private void OnStartGameRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var sessionIdDic = SessionManager.Instance.GetSessionIdDic();
        var startResult = JsonSerializer.Deserialize<StartGameRequest>(buffer);
        if (startResult.Result.Equals("yes", StringComparison.OrdinalIgnoreCase) || startResult.Result.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            if (startResult.Result.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                //게임을 시작하자 
                NotifyStartGame(startResult.RoomNumber);
                return;
            }
            Console.WriteLine("방장이 아직 더 기다리겠답니다..");
            var rooms = RoomManager.Instance.GameRooms;
            
            GameRoom targetRoom = new();
            foreach(var room in rooms)
            {
                if (room.Key == startResult.RoomNumber)
                {
                    targetRoom = room.Value;
                    break;
                }
            }

            foreach (var member in targetRoom.Room)
            {
                if(member?.UserState != UserState.HOST && member?.UserState !=  UserState.SPECTATOR)
                    sessionIdDic[member.SessionId].Send(new MakePacket().NotifyWait());
            }
            return;
        }

        Console.Write("방장이 이상한 값을 보냈다 다시 보내라고 요청하자");
        var gameRooms = RoomManager.Instance.GameRooms;
        if (gameRooms.TryGetValue(startResult.RoomNumber, out GameRoom? gameRoom))
        {
            ushort count = CheckReadyRoom(gameRoom);
            ServerSend(new MakePacket().AskAgainPossibleStartGamePacket(count, gameRoom.RoomId));
        }
    }
}
