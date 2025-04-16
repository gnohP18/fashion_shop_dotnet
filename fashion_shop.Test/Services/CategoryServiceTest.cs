using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Services;
using Moq;

namespace fashion_shop.Test.Services
{
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
        public async Task GetListAsync_ShouldReturnPaginatedData_WhenCalled()
        {
            // Arrange
            var request = new GetCategoryRequest
            {
                Name = "Category1",
                Page = 1,
                Offset = 10,
                SortBy = "Name",
                Direction = "ASC"
            };

            var categories = new List<Category>
            {
                new() { Id = 1, Name = "Category1" },
                new() { Id = 2, Name = "Category2" }
            }.AsQueryable();

            _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(categories);

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

            _mapperMock.Setup(mapper => mapper.Map<Category>(request)).Returns(category);
            _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(new List<Category>().AsQueryable());

            _mapperMock.Setup(mapper => mapper.Map<CreateCategoryResponse>(category))
                .Returns(new CreateCategoryResponse { Id = 1, Name = "NewCategory" });

            // Act
            var result = await _categoryService.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("NewCategory", result.Name);
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Category>(c => c != null)), Times.Exactly(1));
            _categoryRepositoryMock.Verify(repo => repo.UnitOfWork.SaveChangesAsync(), Times.Once(1));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowBadRequestException_WhenNameAlreadyExists()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "ExistingCategory" };

            var categories = new List<Category>
            {
                new() { Id = 1, Name = "ExistingCategory" }
            }.AsQueryable();

            _categoryRepositoryMock.Setup(repo => repo.Queryable).Returns(categories);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.CreateAsync(request));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCategory_WhenValidId()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1" };

            _categoryRepositoryMock.Setup(repo => repo.Queryable)
                .Returns(new List<Category> { category }.AsQueryable());

            _productRepositoryMock.Setup(repo => repo.Queryable)
                .Returns(new List<Product>().AsQueryable());

            // Act
            await _categoryService.DeleteAsync(1);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.UnitOfWork.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.Queryable)
                .Returns(new List<Category>().AsQueryable());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowBadRequestException_WhenCategoryHasProducts()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1" };
            var product = new Product { Id = 1, Name = "Product1", CategoryId = 1 };

            _categoryRepositoryMock.Setup(repo => repo.Queryable)
                .Returns(new List<Category> { category }.AsQueryable());

            _productRepositoryMock.Setup(repo => repo.Queryable)
                .Returns(new List<Product> { product }.AsQueryable());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.DeleteAsync(1));
        }
    }
}