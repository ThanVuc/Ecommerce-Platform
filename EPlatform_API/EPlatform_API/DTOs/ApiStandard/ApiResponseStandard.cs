using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ApiStandard
{
    public class ApiResponseStandard<T>
    {
        public int Status {get; set;}
        public string Message {get; set;} = string.Empty;
        public T? Data {get; set;}
        public List<Link>? Resources {get; set;}
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}