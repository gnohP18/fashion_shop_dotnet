using System.ComponentModel.DataAnnotations;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateVariantRequest
{
    [Required(ErrorMessage = "Priority is required")]
    [Range(1, ProductSettingContants.MaxLengthVariant)]
    public int Priority { get; set; }
    public string Value { get; set; } = default!;
}