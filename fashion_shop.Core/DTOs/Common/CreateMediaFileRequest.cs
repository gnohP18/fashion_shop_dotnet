using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Common
{
    public class CreateMediaFileRequest
    {
        public Stream FileStream { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string FileExtension { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public string ObjectType { get; set; } = default!;
        public int ObjectId { get; set; }
    }
}