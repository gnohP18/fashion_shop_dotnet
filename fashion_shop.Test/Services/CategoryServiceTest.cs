using AutoMapper;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Services;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

public class CategoryServiceTest
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTest()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _categoryService = new CategoryService(
            _categoryRepositoryMock.Object,
            _mapperMock.Object,
            _productRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnPaginatedData_WhenValidRequest()
    {
        // Arrange
        var request = new GetCategoryRequest
        {
            KeySearch = "Category1",
            Page = 1,
            Offset = 10,
            SortBy = "Name",
            Direction = "ASC"
        };

        var categories = new List<Category>
    {
        new() { Id = 1, Name = "Category1" },
        new() { Id = 2, Name = "Category2" }
    };

        var mock = categories.AsQueryable().BuildMockDbSet(); // Supports async operations like CountAsync
        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(mock.Object);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(new List<CategoryDto>
            {
            new() { Id = 1, Name = "Category1" },
            new() { Id = 2, Name = "Category2" }
            });

        // Act
        var result = await _categoryService.GetListAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory_WhenValidRequest()
    {
        // Arrange

        var request = new CreateCategoryRequest { Name = "NewCategory" };
        var category = new Category { Id = 1, Name = "NewCategory" };

        // Giả lập Queryable không có dữ liệu trùng
        _mapperMock.Setup(mapper => mapper.Map<Category>(request)).Returns(category);
        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(new List<Category>().AsQueryable().BuildMockDbSet().Object);

        // Giả lập SaveChangesAsync
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _categoryRepositoryMock.Setup(repo => repo.UnitOfWork).Returns(unitOfWorkMock.Object);


        _mapperMock.Setup(mapper => mapper.Map<CreateCategoryResponse>(category))
            .Returns(new CreateCategoryResponse { Id = 1, Name = "NewCategory" });

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("NewCategory", result.Name);
        _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = string.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "ExistingCategory" };
        var existingCategories = new List<Category>
        {
            new() { Id = 1, Name = "ExistingCategory" }
        }.AsQueryable();

        // Tạo Mock có queryable
        var mockQueryable = existingCategories.BuildMock();
        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(mockQueryable);

        // Mapper
        _mapperMock.Setup(m => m.Map<Category>(request))
            .Returns(new Category { Name = request.Name });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.CreateAsync(request));
        Assert.Equal("Name already existed", ex.Message);
    }

    [Fact]

    public async Task DeleteAsync_ShouldDeleteCategory_WhenValidId()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category1" };

        var categories = new List<Category> { category }.AsQueryable();
        var mockCategoryQueryable = categories.BuildMock(); // 👈 hỗ trợ async
        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(mockCategoryQueryable);

        var products = new List<Product>().AsQueryable(); // Không có product nào liên quan
        var mockProductQueryable = products.BuildMock();
        _productRepositoryMock.Setup(repo => repo.Queryable).Returns(mockProductQueryable);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _categoryRepositoryMock.Setup(repo => repo.UnitOfWork).Returns(unitOfWorkMock.Object);

        // Act
        await _categoryService.DeleteAsync(1);

        // Assert
        _categoryRepositoryMock.Verify(repo => repo.Delete(It.Is<Category>(c => c.Id == 1)), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var emptyCategories = new List<Category>().AsQueryable();
        var mockCategoryQueryable = emptyCategories.BuildMock(); // Hỗ trợ async
        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(mockCategoryQueryable);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.DeleteAsync(1));
    }


    [Fact]
    public async Task DeleteAsync_ShouldThrowBadRequestException_WhenCategoryHasProducts()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category1" };
        var product = new Product { Id = 1, Name = "Product1", CategoryId = 1 };

        var mockCategoryQueryable = new List<Category> { category }.AsQueryable().BuildMock();
        var mockProductQueryable = new List<Product> { product }.AsQueryable().BuildMock();

        _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(mockCategoryQueryable);
        _productRepositoryMock.Setup(repo => repo.Queryable).Returns(mockProductQueryable);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.DeleteAsync(1));
    }

}
