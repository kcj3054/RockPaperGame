using StackExchange.Redis;

namespace LoginServer.Redis
{
    public class Redis
    {
        ConnectionMultiplexer _redisConn;
        IDatabase _redisDb;

        public Redis()
        {
            _redisConn = ConnectionMultiplexer.Connect("localhost");
            _redisDb = _redisConn.GetDatabase();
        }

        public async Task<bool> StoreAccount(string userID, string value)
        {
            try
            {
                var result = await _redisDb.SetAddAsync(userID, value);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> RemoveAccount(string userID)
        {
            try 
            {
                var result = await _redisDb.KeyDeleteAsync(userID);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }            
        }

        public string? GetValue(string key)
        {
            return _redisDb.StringGet(key);
        }

       
       
      

    }
}
