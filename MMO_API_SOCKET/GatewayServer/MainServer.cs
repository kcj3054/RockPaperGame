using Microsoft.Extensions.Hosting;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;

namespace GameServer;

// AppServer가 supersocket MainServer 틀 
// Appserver의 start 함수가 서버를 start해준다 
public class MainServer : AppServer<NetWorkSession, EFBinaryRequestInfo> , IHostedService
{
    //?
    private readonly IHostApplicationLifetime _appLifeTime;
    private readonly ServerOption _serverOption;
    
    
    //
    public static SuperSocket.SocketBase.Logging.ILog MainLog;

    private Dictionary<int, Action<NetWorkSession, EFBinaryRequestInfo>> HandlerMap = new();
    // private CommandHandler CommandHandler = new();
    
    
    private IServerConfig m_Config;
    
    public MainServer()
        : base()
    {
        NewSessionConnected += new SessionHandler<NetWorkSession>(OnConnected);
        SessionClosed += new SessionHandler<NetWorkSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<NetWorkSession, EFBinaryRequestInfo>(RequestReceived);
    }

    public void InitConfig(ServerOption option)
    {
        m_Config = new ServerConfig()
        {

        };
    }

    public void CreateServer()
    {
        try
        {
            Setup(new RootConfig(), m_Config, logFactory: new NLogLogFactory());
            
            RegisterHandler();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void RegisterHandler()
    {
        // HandlerMap.Add((int)PacketID.REQ_ECHO, CommandHandler.RequestEcho);
        
        //handler 등록 
    }
    private void RequestReceived(NetWorkSession session, EFBinaryRequestInfo requestinfo)
    {
        if (HandlerMap.ContainsKey(requestinfo.PacketID))
        {
            HandlerMap[requestinfo.PacketID](session, requestinfo);
        }
        else
        {
            // MainLog.Info();
        }
    }

    // Supersocket내부에서는 thread pool을 사용하고있다, 
    // ~ -> RequestReceived -> OnPacketReceivec인가 ? 확인  
    private void OnPacketReceivec(ClientSession session, EFBinaryRequestInfo reqinfo)
    {
        //session 받은 데이터 크기 .. 
    }
    //
    // public void Distribute(ServerPacketData requestPacket)
    // {
    //     MainPacketProcessor.InsertPacket(requestPacket);
    // }

    private void OnClosed(NetWorkSession session, CloseReason value)
    {
        MainLog.Info($"{session} Closed");
    }

    private void OnConnected(NetWorkSession session)
    {
        MainLog.Debug($"{session} connected");
    }
    
    //Generic Hosting
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //todo :  ! 공부하기 
        _ = _appLifeTime.ApplicationStarted.Register(AppOnStarted);
        _ = _appLifeTime.ApplicationStopped.Register(AppOnStoped);
        return Task.CompletedTask;
    }

    private void AppOnStarted()
    {
        
    }

    private void AppOnStoped()
    {
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

internal class ClientSession
{
}

public class NetWorkSession : AppSession<NetWorkSession, EFBinaryRequestInfo>
{
    
}