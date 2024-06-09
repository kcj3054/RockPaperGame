namespace Common;

public class Room
{
    public ushort RoomId { get; set; }
    public bool IsRun { get; set; }

    //몇명인지만 체크하면되지않을까? 
    public int Count { get; set; }
}