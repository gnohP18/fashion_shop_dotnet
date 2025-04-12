using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Common
{
    public class MediaFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = default!;
        public string FileExtension { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}