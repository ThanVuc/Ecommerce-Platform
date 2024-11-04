using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EPlatform_API.ExtensionMethods
{
    public static class DistributedCacheExtensions
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings{
            ContractResolver = new DefaultContractResolver{
                NamingStrategy = null
            },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value){
            return SetAsync(cache,key,value, new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1)));
        }

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options){
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value,jsonSettings));
            return cache.SetAsync(key,bytes,options);
        }

        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value){
            var val = cache.Get(key);
            value = default;
            if (val == null) return false;
            value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(val),jsonSettings);
            return true;
        }

        public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, T val, DistributedCacheEntryOptions? options = null){
            if (options == null){
                options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            if (cache.TryGetValue(key,out T? value) && value is not null){
                return value;
            }
            
            if (val is not null){
                await cache.SetAsync<T>(key,val,options);
            }
            return value;
        }

        public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task, DistributedCacheEntryOptions? options = null){
            if (options == null){
                options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            if (cache.TryGetValue(key,out T? value) && value is not null){
                return value;
            }

            value = await task();
            
            if (value is not null){
                await cache.SetAsync<T>(key,value,options);
            }
            return value;
        }

        

    }
}