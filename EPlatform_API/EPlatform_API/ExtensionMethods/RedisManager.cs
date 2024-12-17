using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EPlatform_API.ExtensionMethods
{
    public class RedisManager
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        static RedisManager()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
                var options = new ConfigurationOptions{
                    EndPoints = { "localhost:6379" },
                    ConnectTimeout = 5000,
                    SyncTimeout = 10000,
                    AbortOnConnectFail = false
                };
                return ConnectionMultiplexer.Connect(options);
            });
        }
        public static ConnectionMultiplexer Connection => lazyConnection.Value;
    }
}