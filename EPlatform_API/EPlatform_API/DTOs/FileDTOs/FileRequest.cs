using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.FileDTOs
{
    public class FileRequest
    {
        public string Name {get; set;}
        public IFormFile File {get; set;}
    }
}