namespace Common;

public class Singleton<T> where T : class, new()
{
    private static T? _instance = null;
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Interlocked.CompareExchange(ref _instance, new T(), null);
            
            return _instance;
        }
        
    }
}

public enum SingletonEnum {
    INSTANCE
}