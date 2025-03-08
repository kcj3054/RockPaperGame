using Common;

namespace Client.Service;

public class ClientGameManager : IClientGameManager
{
    // /ready request를 보내야한다.. 
    public bool RestartGame()
    {
        while (true)
        {
            Console.WriteLine("현재 게임이 종료되었습니다. 다음 게임에 참가하실 건가요?? (yes or no로 대답해주세요.)\n\n");
            var cmd = Console.ReadLine();

            switch (cmd)
            {
                case string command  when !(command is "yes" or "no"):
                    Console.WriteLine("yes or no를 안했으니 한번 더 입력해주세요\n\n ");
                    break;
                case string cmmand when cmmand is "no":
                    return false;
                default:
                    return true;
            }
        }
    }

    public void Attack( string nickName)
    {
        //만약 방의 유저들 중 
        Console.WriteLine($"플레이어님 1(가위), 2(바위), 3(보) 중 하나를 결정해주세요\n\n");
        var cmd = Console.ReadLine();
            
        ClientSession.Instance.Send(new MakePacket().AttackReqPacket(nickName, cmd));
    }
}