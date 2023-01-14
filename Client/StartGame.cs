using Common;
namespace Client;
public class StartGame
{ 
    public void Start()
    {
        //todo : 나중에 수정하자 
        Console.WriteLine("게임을 시작하겠습니다 !  소문자로 yes를 누르면 게임이 시작됩니다\n다른 숫자를 것을 입력하면 종료됩니다.");
        if (Console.ReadLine() is "yes")
        {
            InputPlayerId();
            OnShowRoom();
        }
        else
        {
            Console.WriteLine("종료 되었습니다 다시 접속해주세요.");
            ClientSession.Instance.Disconnect();
        }
        
        while (true)
        {
            ;
        }
    }

    private void InputPlayerId()
    {
        while (true)
        {
            Console.WriteLine("원하시는 플레이어 아이디를 적어주세요 (10글자 미만, 공백제외)");
            var input = Console.ReadLine();
            VerifyPlayerId(input);
            
            Thread.Sleep(500);
            //진짜인지 아닌지 확인하고 break
            if (ClientHandler.Instance.GetPlayerIdOk())
            {
                Console.WriteLine($"정상적으로 처리되었습니다 당신의 아이디는 {input}");
                
                ClientHandler.Instance.SetPlayerNickname(input);
                break;
            }
            Console.WriteLine("다시 입력해주세요");
        }
    }

    private void VerifyPlayerId(string playerNickName)
    {
        ClientSession.Instance.Send(new MakePacket().VerifyPlayerId(playerNickName));
    }

    private void OnShowRoom()
    {
        ClientSession.Instance.Send(new MakePacket().
            ShowRoomReqPacket(ClientHandler.Instance.GetPlayerNickname()));
    }
}