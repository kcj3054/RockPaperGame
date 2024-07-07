using System.Text.Json;
using Common;
using ServerCommon;
using static Common.RockPaper;
using static ServerCommon.UserState;

namespace Server;

public partial class ServerHandler
{
    private void OnVerifyPlayerId(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        bool isValid = true;
        var verifyIdRequest = JsonSerializer.Deserialize<VerifyIdRequest>(buffer);
        var nickName = verifyIdRequest.NickName;

        var playerIdDic = SessionManager.Instance.GetUserDic();
        var nicknameDic = SessionManager.Instance.GetNicknameDic();


        if (string.IsNullOrEmpty(nickName) || nickName.Length > (int)Name.MaxLength)
        {
            isValid = false;
        }

        if (nicknameDic.TryGetValue(nickName, out _))
        {
            isValid = false;
        }


        if (playerIdDic.ContainsKey(_serverSession) == false && isValid)
        {
            playerIdDic.TryAdd(
                _serverSession,
                new User()
                {
                    UserState = PLAYER, NickName = nickName, AttackValue = (ushort)None,
                    SessionId = _serverSession.sessionId
                });
            nicknameDic.TryAdd(nickName, true);
        }
        ServerSend(new MakePacket().VerifyPlayerIdResPacket(isValid));
    }
}