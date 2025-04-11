using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Mappers;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<CreateProductRequest, Product>();
    }
}