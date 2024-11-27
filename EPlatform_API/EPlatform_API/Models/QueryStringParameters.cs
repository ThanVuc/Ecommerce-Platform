using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public abstract class QueryStringParameters : PagingQueryStringParamaters
    {
        public string? SearchString {get; set;}
    }
}