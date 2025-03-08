using MessagePack;

namespace GameServer;

public class PacketData
{
    
}
//MessagePack이 c# 직렬화 라이브러리에서 가장 성능이 우승하고, c# signal R에서도 내부적으로 사용 중 임 
[MessagePackObject]
public class PKTRequLogin
{
    [Key(0)] public string UserID;
    [Key(1)] public String AuthToken;
}

[MessagePackObject]
public class PKTResLogin
{
    
}