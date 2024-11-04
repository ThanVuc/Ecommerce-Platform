using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ApiStandard
{
    public class DataStandard<T>
    {
        public T? Data {get; set;}
        public Dictionary<string,Link> Resources {get; set;} = new Dictionary<string, Link>();
    }
}