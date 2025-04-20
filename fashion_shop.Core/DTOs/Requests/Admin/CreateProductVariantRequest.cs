using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateProductVariantRequest
{
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(10, ErrorMessage = "Maxlength is 10 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Priority is required")]
    [Range(1, ProductSettingContants.MaxLengthProductVariant)]
    public int Priority { get; set; }

}