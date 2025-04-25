using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;
public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Slug is required")]
    [MinLength(3, ErrorMessage = "Slug must be at least 3 characters")]
    public string Slug { get; set; } = default!;
}