using Microsoft.AspNetCore.Connections.Features;

namespace APIServer.Controllers;

/*
 * Mail을 지급하는 것은 보통 운영툴을 이용해서 보상을 지급한다. or 일정 퀘스트를 달성 시 보상이 지급된다.  
 */
public interface IMailService
{
    // public Task<()> GetMailList(int uid);

    // public Task<(ErrorCode, List<ReceivedReward>) ReceiveMail(int uid, int mailSeq);
    public Task<ErrorCode> DeleteMail(int userID, int mailID);
}