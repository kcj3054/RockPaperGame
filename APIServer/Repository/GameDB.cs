namespace APIServer.Repository;

public class GameDB : IGameDB
{
    public Task<int> DeleteMail(int mailID)
    {
        //mailbox에서 메일 제거 
        //

        return (Task<int>)Task.CompletedTask;
    }
}