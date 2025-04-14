using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Interfaces.Services;

public interface ICartService
{
    Task<Dictionary<ProductDto, int>> GetListAsync(Dictionary<int, int> cartItems);
}