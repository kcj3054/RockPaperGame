namespace Server;

public partial class ServerHandler
{
    private void OnPlayerExitRequest(int packetHeaderInfo, ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;
        
        SessionManager.Instance.DisappearUser(_serverSession.sessionId);
    }
}