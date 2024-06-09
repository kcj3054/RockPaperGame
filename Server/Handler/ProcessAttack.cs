using Google.Protobuf;
using System.Text.Json;
using Common;
using ServerCommon;
using static Common.RockPaper;

namespace Server;

public partial class ServerHandler
{
    private void OnPlayerAttackRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var attackRequest = new AttackRequest.Parser.ParseFrom(buffer.Array, buffer.Offset, buffer.Count);
        // var attackRequest = JsonSerializer.Deserialize<AttackRequest>(buffer);
        var gameRooms = RoomManager.Instance.GameRooms;
        var playerDic = SessionManager.Instance.GetUserDic();
        
        if (ushort.TryParse(attackRequest?.Value, out var value) && ((ushort)SCISSORS <= value && value <= (ushort)PAPER))
        {
            if (playerDic.TryGetValue(_serverSession, out User? user))
            {
                if(gameRooms.TryGetValue(user.RoomId, out GameRoom? roomm))
                {
                    Console.WriteLine($"{attackRequest.Nickname}님이 내신 값은 {attackRequest.Value}입니다");
                    switch (value)
                    {
                        case (ushort)SCISSORS:
                            user.AttackValue = (ushort)SCISSORS;
                            break;
                        case (ushort)ROCK:
                            user.AttackValue = (ushort)ROCK;
                            break;
                        case (ushort)PAPER:
                            user.AttackValue = (ushort)PAPER;
                            break;
                    }
                    if (roomm.Room.Count == (int)UserCount.Alone)
                    {
                        Console.WriteLine($"{attackRequest.Nickname}님이 홀로 남으셨습니다");
                        return;
                    }
                    if (CheckRoomMemberValue(roomm) && CalculateRoomPlayUserCount(roomm) >= (int)UserCount.Min)
                    {
                        DecideGame(roomm);
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("클라이언트가 이상합니다 이상한 값을 공격값으로 내고있습니다");
            ServerSend(new MakePacket().NoifyAttackAgainPacket());
        }
    }
}