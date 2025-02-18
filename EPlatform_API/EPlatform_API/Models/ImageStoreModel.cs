using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
        public class ImageStoreModel
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool isUpdating { get; set; } = false;
    }
}