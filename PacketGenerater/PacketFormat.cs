namespace PacketGenerater;

public class PacketFormat
{
    // {0} 패킷 이리ㅡㅁ
    // {1} 맴버 변수들 
    //{2} 맴버 변수 read
    // {3} 맴버 변수 write..
    
    public static string packetFormat =
        @"
        class {0}
        {{
            {1}
            
        }}
        ";
    
    // {0} 변수의 형식
    // {1} 변수 이름 
    public static string memberFormat =
        @"public {0} {1}";

    //{0} 변수 이름 
    // {1} To~ 변수형식
    // {2} 변수형식 
    public static string readFormat =
        @"";
}