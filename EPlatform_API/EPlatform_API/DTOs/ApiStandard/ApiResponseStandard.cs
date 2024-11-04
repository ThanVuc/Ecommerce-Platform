using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ApiStandard
{
    public class ApiResponseStandard<T>
    {
        public string Status {get; set;} = string.Empty;
        public string Message {get; set;} = string.Empty;
        public T? Data {get; set;}
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public Dictionary<string,string>? Errors {get; set;}
    }
}