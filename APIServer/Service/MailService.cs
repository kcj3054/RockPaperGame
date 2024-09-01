using APIServer.Repository;

namespace APIServer.Controllers;

public class MailService : IMailService
{
    private IGameDB _gameDB;

    public MailService(IGameDB gameDB)
    {
        _gameDB = gameDB;
    }
    
    public Task<ErrorCode> DeleteMail(int userID, int mailID)
    {
        try
        {
            //메일 유무 확인 
            //get mail info 등등// mail함에서 mail이 존재하는 지 확인
            
            //메일 삭제 
            
            //메일 보상 삭제

            return  (Task<ErrorCode>)Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}