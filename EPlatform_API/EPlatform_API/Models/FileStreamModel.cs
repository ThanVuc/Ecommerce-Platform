using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public class FileStreamModel
    {
        public string? Name {get; set;}
        public Stream? Stream {get; set;}
    }
}