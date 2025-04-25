using AutoMapper;
using AutoMapper.QueryableExtensions;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services
{
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly IMapper _mapper;
        private MinioSettings _minioSettings;

        public ProductItemService(IProductItemRepository productItemRepository, IMapper mapper, IOptions<MinioSettings> options)
        {
            _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(productItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        }

        public async Task<ProductItemDto?> GetDetailAsync(int id)
        {
            return await _productItemRepository
                .Queryable
                .WithoutDeleted()
                .ProjectTo<ProductItemDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductItemDto>> GetListProductItemByProductId(int productId)
        {
            var data = await _productItemRepository
                .Queryable
                .WithoutDeleted()
                .Where(p => p.ProductId == productId)
                .OrderBy(p => p.Id)
                .ProjectTo<ProductItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            data.ForEach(element =>
            {
                element.ImageUrl = !string.IsNullOrEmpty(element.ImageUrl) ? $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{element.ImageUrl}" : string.Empty;
            });

            return data;
        }
    }
}