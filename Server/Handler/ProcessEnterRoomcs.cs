using System.Text.Json;
using Common;
using Server.Service;
using static ServerCommon.UserState;

namespace Server;

public partial class ServerHandler
{
    private void OnEnterRoomRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        var enterRoomRequest = JsonSerializer.Deserialize<EnterRoomRequest>(buffer);

        if (enterRoomRequest == null)
            return;

        bool flag = false;
        var rooms = RoomManager.Instance.GameRooms;
        if (rooms.ContainsKey((ushort)enterRoomRequest.RoomNmber))
        {
            flag = true;
        }

        if (!flag && enterRoomRequest.RoomNmber != -1)
        {
            ServerSend(new MakePacket().NotEixtRoomNumberPacket());
            return;
        }

        Console.WriteLine($"{enterRoomRequest.NickName}님이  {enterRoomRequest.RoomNmber}번방에 입장하기를 원합니다");
        (var num, var userState) = GameManager.Instance.EnterRoom(_serverSession, enterRoomRequest.RoomNmber);

        switch (userState)
        {
            case var state when state == PLAYER || state == SPECTATOR:
                ServerSend(new MakePacket().EnterRoomResPacket(num, (ushort)userState));
                break;
            case HOST:
                ServerSend(new MakePacket().EnterRoomResToHostPacket(num));
                break;
        }
    }
}