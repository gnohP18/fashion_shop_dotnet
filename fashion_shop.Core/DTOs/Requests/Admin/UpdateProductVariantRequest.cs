using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class UpdateProductVariantRequest
{
    public bool IsVariant { get; set; }
    public List<UpdateProductItemRequest> ProductItems { get; set; } = new List<UpdateProductItemRequest>();
    public List<CreateProductVariantRequest> ProductVariants { get; set; } = new List<CreateProductVariantRequest>();
    public List<CreateVariantRequest> Variants { get; set; } = new List<CreateVariantRequest>();
}