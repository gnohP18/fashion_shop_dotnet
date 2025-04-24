using System.ComponentModel.DataAnnotations;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateVariantRequest
{
    [Required(ErrorMessage = "Priority is required")]
    [Range(1, ProductSettingContants.MaxLengthVariant)]
    public int Priority { get; set; }

    [Required(ErrorMessage = "Value is required")]
    public string Value { get; set; } = default!;


    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = default!;
}