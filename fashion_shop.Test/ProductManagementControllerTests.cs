using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.API.Controllers.Admin;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace fashion_shop.Tests.Controllers;

public class ProductManagementControllerTests
{
    public static IEnumerable<object[]> GetProductTestCases()
    {
        yield return new object[]
        {
                new GetProductRequest
                {
                    KeySearch = "Black suit",
                    Page = 1,
                    Limit = 15,
                    Direction = "DESC",
                    SortBy = "Id"
                },
                new PaginationData<ProductDto>(
                    data: [],
                    pageSize: 15,
                    currentPage: 1,
                    total: 1
                )
        };

        yield return new object[]
        {
                new GetProductRequest
                {
                    Page = 2,
                    Limit = 5,
                    Direction = "ASC",
                    SortBy = "Name"
                },
                new PaginationData<ProductDto>(
                    new List<ProductDto>
                    {
                        new() { Id = 2, Name = "Product B", Slug = "product-b", Price = 200000, ImageUrl = "img.jpg", CategoryId = 2, CategoryName = "Category B" }
                    },
                    pageSize: 5,
                    currentPage: 2,
                    total: 6
                )
        };
    }

    [Theory]
    [MemberData(nameof(GetProductTestCases))]
    public async Task GetAllProductFromController_ShouldReturnExpectedResult(
        GetProductRequest request,
        PaginationData<ProductDto> expected)
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var mockProductService = new Mock<IProductService>();
        var mockLogger = new Mock<ILogger<ProductManagementController>>();

        mockProductService
            .Setup(x => x.GetListAsync(It.Is<GetProductRequest>(r => r.KeySearch == "valid")))
            .ReturnsAsync(expected);

        var controller = new ProductManagementController(
            mockLogger.Object,
            mockCategoryService.Object,
            mockProductService.Object
        );

        // Act
        var result = await controller.GetProductAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Total, result.Total);
        Assert.Equal(expected.PageSize, result.PageSize);
        Assert.Equal(expected.CurrentPage, result.CurrentPage);
        Assert.Equal(expected.Data.Count(), result.Data.Count());
    }
}
