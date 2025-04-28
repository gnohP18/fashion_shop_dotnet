using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services
{
    public partial class ProductService
    {
        public async Task CreateAsync(CreateProductRequest request)
        {
            var category = await _categoryRepository
                .Queryable
                .WithoutDeleted()
                .FirstOrDefaultAsync(_ => _.Id == request.CategoryId);

            if (category is null)
            {
                throw new NotFoundException($"Not found category Id={request.CategoryId}");
            }

            var product = new Product()
            {
                Name = request.Name,
                Slug = Core.Common.Function.GenerateSlugProduct(request.Slug),
                Price = request.Price,
                Description = request.Description,
                CategoryId = category.Id,
                IsVariant = request.IsVariant,
                ImageUrl = request.ImageUrl,
            };

            if (request.IsVariant)
            {
                if (request.ProductVariants.Count > 0 && request.Variants.Count > 0)
                {
                    // Handle create variant
                    var productVariants = GenerateProductVariant(request.ProductVariants, request.Variants);

                    product.ProductVariants = productVariants;

                    var variantGroups = GenerateCombinations(productVariants.Select(p => p.Variants.Select(v => v.Code).ToList()).ToList());

                    product.ProductItems = PrepareProductItemData(variantGroups, request.Variants);
                }
            }
            else
            {
                // we create only one productItem
                product.ProductItems = new HashSet<ProductItem>()
                {
                    new ProductItem
                    {
                        Code = "_",
                        Price = request.Price,
                        ImageUrl = "",
                        Quantity = 0,
                        VariantObjects = new List<VariantObject>()
                    }
                };
            }

            await _productRepository.AddAsync(product);
            await _productRepository.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Generate product variant
        /// </summary>
        /// <param name="productVariants">List Product Variant</param>
        /// <param name="variants">List Variant of Product Variant</param>
        /// <returns>Hashset ProductVariant</returns>
        private HashSet<ProductVariant> GenerateProductVariant(
            List<CreateProductVariantRequest> productVariants,
            List<CreateVariantRequest> variants)
        {
            productVariants = productVariants.OrderBy(p => p.Priority).ToList();

            var newProductVariants = new HashSet<ProductVariant>();

            var dataVariants = new HashSet<Variant>();

            productVariants.ForEach(productVariant =>
            {
                var selectedVariant = variants.Where(v => v.Priority == productVariant.Priority).ToList();

                // Skip if not fount variant
                if (selectedVariant.Count == 0)
                {
                    return;
                }

                var data = PrepareVariantData(selectedVariant);

                dataVariants.Concat(data);
                newProductVariants.Add(new ProductVariant()
                {
                    Name = productVariant.Name,
                    Priority = productVariant.Priority,
                    Variants = data.ToHashSet()
                });
            });

            HasDuplicateCode(dataVariants);

            return newProductVariants;
        }

        /// <summary>
        /// Prepare Variant Data
        /// </summary>
        /// <param name="variants">Variant Data</param>
        /// <returns></returns>
        private HashSet<Variant> PrepareVariantData(List<CreateVariantRequest> variants)
        {
            return variants.Select(v => new Variant()
            {
                Value = v.Value,
                Code = v.Code.ToLower()
            }).ToHashSet();
        }

        /// <summary>
        /// Generate combination Data
        /// </summary>
        /// <param name="variantGroups">List of List string Code</param>
        /// <returns></returns>
        public List<string> GenerateCombinations(List<List<string>> variantGroups)
        {
            var results = new List<string>();

            GenerateRecursive(variantGroups, 0, new List<string>(), results);

            return results;
        }

        /// <summary>
        /// Tạo đệ qui quay lui cho thêm list variant
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="index"></param>
        /// <param name="current"></param>
        /// <param name="result"></param>
        private void GenerateRecursive(
            List<List<string>> groups,
            int index,
            List<string> current,
            List<string> result)
        {
            if (index == groups.Count)
            {
                result.Add(string.Join("_", current));
                return;
            }

            foreach (var value in groups[index])
            {
                current.Add(value);
                GenerateRecursive(groups, index + 1, current, result);
                current.RemoveAt(current.Count - 1); // backtrack
            }
        }

        /// <summary>
        /// Prepare data for ProductItem
        /// </summary>
        /// <param name="productItemCodes">Code</param>
        /// <returns>HashSet ProductItem</returns>
        private HashSet<ProductItem> PrepareProductItemData(List<string> productItemCodes, List<CreateVariantRequest> variantRequests)
        {
            var data = new HashSet<ProductItem>();

            productItemCodes.ForEach(productItemCode =>
            {
                var variantObjects = new List<VariantObject>();

                productItemCode.Split("_").ToList().ForEach(variantCode =>
                {
                    variantObjects.Add(
                        variantRequests.Where(vairant => vairant.Code == variantCode)
                        .Select(v => new VariantObject
                        {
                            Value = v.Value,
                            Code = v.Code
                        }).First());
                });

                data.Add(new ProductItem
                {
                    Code = productItemCode,
                    Price = 0,
                    ImageUrl = "",
                    Quantity = 0,
                    VariantObjects = variantObjects
                });
            });

            return data;
        }

        /// <summary>
        /// Update Basic Info Product
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="request">Info Product</param>
        /// <returns></returns>
        public async Task UpdateBasicInfoAsync(int id, UpdateProductRequest request)
        {
            var product = await _productRepository
                .Queryable
                .WithoutDeleted()
                .FirstAsync(p => p.Id == id);
            var oldSlug = ExtractPrefix(product.Slug);

            var productMapper = _mapper.Map(request, product);

            if (!string.Equals(oldSlug, request.Slug, StringComparison.OrdinalIgnoreCase))
            {
                productMapper.Slug = Core.Common.Function.GenerateSlugProduct(request.Slug);
            }
            else
            {
                request.Slug = product.Slug; // Giữ nguyên slug cũ
            }

            if (!product.IsVariant)
            {
                var productItem = await _productItemRepository.Queryable.Where(i => i.ProductId == product.Id).FirstAsync();

                productItem.Price = productMapper.Price;
            }

            _productRepository.Update(productMapper);
            await _productRepository.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Update ProductVariant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task UpdateProductVariantAsync(int id, UpdateProductVariantRequest request)
        {
            var product = await _productRepository
                .Queryable
                .WithoutDeleted()
                .FirstOrDefaultAsync(p => p.Id == id);

            var prevState = product?.IsVariant;
            var nextState = request.IsVariant;

            // Case prev: True -> next: False -> Disable
            if (prevState is true && nextState is false)
            {
                await DisableProductVariant(id);
                if (product is not null)
                {
                    product.IsVariant = request.IsVariant;
                    // After disabled all product variant, we create only one productItem
                    _productRepository.Update(product);
                }

                await _productRepository.UnitOfWork.SaveChangesAsync();

                return;
            }

            // Case prev: False -> next: True -> Create New Variant
            if (prevState is false && nextState is true)
            {
                var productVariants = GenerateProductVariant(request.ProductVariants, request.Variants);
                if (product is not null)
                {
                    await DisableProductVariant(id);
                    product.ProductVariants = productVariants;

                    var variantGroups = GenerateCombinations(productVariants.Select(p => p.Variants.Select(v => v.Code).ToList()).ToList());

                    product.ProductItems = PrepareProductItemData(variantGroups, request.Variants);

                    product.IsVariant = request.IsVariant;
                    _productRepository.Update(product);
                }

                await _productRepository.UnitOfWork.SaveChangesAsync();

                return;
            }

            // Case prev: True -> next: True -> Update Product Item
            if (prevState is true && nextState is true)
            {
                var productItems = _mapper.Map<List<UpdateProductItemRequest>, List<ProductItem>>(request.ProductItems);

                _productItemRepository.UpdateManySelective(productItems,
                    x => x.Quantity,
                    x => x.Price
                );

                await _productRepository.UnitOfWork.SaveChangesAsync();

                return;
            }
        }

        /// <summary>
        /// For case State prev: True -> next: False
        /// </summary>
        /// <param name="productId">Product</param>
        /// <returns></returns>
        private async Task DisableProductVariant(int productId)
        {
            var productVariants = await _productVariantRepository
                .Queryable
                .WithoutDeleted()
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            var productVariantIds = productVariants.Select(p => p.Id).ToList();

            var variants = await _variantRepository
                .Queryable
                .WithoutDeleted()
                .Where(v => productVariantIds.Contains(v.ProductVariantId))
                .ToListAsync();

            var productItems = await _productItemRepository
                .Queryable
                .WithoutDeleted()
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            _productVariantRepository.DeleteMany(productVariants);
            _variantRepository.DeleteMany(variants);
            _productItemRepository.DeleteMany(productItems);
        }

        /// <summary>
        /// Extract Slug Ex: ao-thun-11231231 -> ao-thun
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        private string ExtractPrefix(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return "";

            var lastDash = slug.LastIndexOf("-");
            if (lastDash == -1) return slug;

            var suffix = slug[(lastDash + 1)..];

            return long.TryParse(suffix, out _) ? slug[..lastDash] : slug;
        }

        /// <summary>
        /// Check dulicate code
        /// </summary>
        /// <param name="variants">variants</param>
        /// <returns>true|false</returns>
        private bool HasDuplicateCode(HashSet<Variant> variants)
        {
            var dulicateCodes = variants
                .GroupBy(v => v.Code)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (dulicateCodes.Any())
            {
                throw new BadRequestException("Duplicate Code: " + string.Join(", ", dulicateCodes));
            }

            return true;
        }
    }

}