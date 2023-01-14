namespace Server;

public interface IGameRoomService
{
    public void Enter(User user);
    public void Leave(User user);
}