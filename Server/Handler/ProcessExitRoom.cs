using System.Text.Json;
using Common;

namespace Server;

public partial class ServerHandler
{
    // private void OnExitRoom(int packetHeaderInfo, ArraySegment<byte> buffer)
    // {
    //     
    //     var attackRequest = JsonSerializer.Deserialize<ExitRoomRequest>(buffer);
    //     SessionManager.Instance.RemoveOrderNumber(attackRequest);
    //      SessionManager.Instance.LeveRoom();
    //     
    //     Send roomList..
    // }
}