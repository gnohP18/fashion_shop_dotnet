using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services
{
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly IMapper _mapper;

        public ProductItemService(IProductItemRepository productItemRepository, IMapper mapper)
        {
            _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(productItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductItemDto?> GetDetailAsync(int id)
        {
            return await _productItemRepository
                .Queryable
                .WithoutDeleted()
                .ProjectTo<ProductItemDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}