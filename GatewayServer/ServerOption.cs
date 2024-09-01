namespace GameServer;

public class ServerOption
{
    public String Name { get; set; }

    public int MaxConnection { get; set; } 
    
    public int Port { get; set; }
    
    public int ReceiveBufferSize { get; set; }
    
    public int SendBufferSize { get; set; }
    
    public String RedisAddress { get; set; }
}