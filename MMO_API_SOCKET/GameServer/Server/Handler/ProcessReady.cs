using System.Text.Json;
using Common;
using static Common.RockPaper;
using static ServerCommon.UserState;

namespace Server;

//해당 session에 맞은 user를 건드리는 것이니 고유의 session의 user를 건드려서 따로 lock은 필요없.. 
public partial class ServerHandler
{
     private void OnReadyRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var readyResponse = JsonSerializer.Deserialize<ReadyRequest>(buffer);
        var playerIdDic = SessionManager.Instance.GetUserDic();
        var gameRooms = RoomManager.Instance.GameRooms;
        if (playerIdDic.TryGetValue(_serverSession, out User? user))
        {
            Console.WriteLine($"{user.NickName}님의 응답은 {readyResponse?.ReadyState}입니다 ");
            if (readyResponse == null)
                return;

            if (readyResponse.ReadyState != "yes" && readyResponse.ReadyState != "no")
            {
                Console.WriteLine("유저가 제대로 입력을 하지않았습니다 다시 요청을 하자.. ");
                ServerSend(new MakePacket().NotifyReadyPacket(user.RoomId));
                return;
            }
            // 플레이어는 게임할 준비가 되었다  , 이 순간에 이미 방장이 게임을 시작했다면.. 해당 유저는 관전자로 돌려주자.. 
            if (readyResponse.ReadyState == "yes")
            {
                // 이미 진행 중인 경우 ..
                if (gameRooms.TryGetValue(user.RoomId, out GameRoom? room))
                {
                    if (room.IsRun)
                    {
                        Console.WriteLine(
                            $"{readyResponse.NickName}님이 준비를 했지만 현재 해당 {room.RoomId}번 룸은 게임이 진행중입니다..\n 유저의 상태를 관전자로 변경합니다");
                        ConvertUserState(user);
                        ServerSend(new MakePacket().NotifyConvertUserState());
                        return;
                    }
                }
                Console.WriteLine($"{readyResponse.NickName}님이 준비를하셨습니다..");

                if (user.UserState != HOST)
                    user.UserState = Ready;

                if (gameRooms.TryGetValue(user.RoomId, out GameRoom? rooms))
                {
                    CheckReadyRoom(rooms);
                }
            }
            else if (readyResponse.ReadyState == "no")
            {
                Console.WriteLine($"{readyResponse.NickName}님이 거부하셨습니다.. 강퇴하겠습니다..");
                MemberExit(readyResponse.NickName, readyResponse.RoomId);

                ServerSend(new MakePacket().NotifyExitPacket());
            }
        }

    }
}