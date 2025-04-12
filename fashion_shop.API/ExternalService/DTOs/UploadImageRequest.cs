using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace fashion_shop.API.ExternalService.DTOs;

public class UploadImageRequest
{
    [Required]
    public IFormFile File { get; set; } = default!;
}