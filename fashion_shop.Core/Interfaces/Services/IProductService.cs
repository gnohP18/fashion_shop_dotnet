using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task AddAsync(Product product);
    }
}