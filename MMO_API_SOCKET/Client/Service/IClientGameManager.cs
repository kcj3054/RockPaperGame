namespace Client.Service;

public interface IClientGameManager
{
    public bool RestartGame();

    public void Attack(string nickName);
}