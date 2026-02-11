using Application.Interfaces;
using Application.UseCases.CreateProduct;
using Bogus;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases;

[Collection("Culture collection")]
public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_On_Success()
    {
        // Arrange
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

        var handler = new CreateProductHandler(repoMock.Object);

        var cmd = new CreateProductCommand
        {
            Name = name,
            Description = description,
            Price = price,
            Category = category
        };

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<CreateProductResponse>(result.Value);
        repoMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.Name == name && p.Description == description && p.Price == price && p.Category == category), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Price_Invalid()
    {
        var repoMock = new Mock<IProductRepository>();
        var handler = new CreateProductHandler(repoMock.Object);

        var cmd = new CreateProductCommand
        {
            Name = "Valid Name",
            Description = "Valid",
            Price = 0m,
            Category = ProductCategory.Electronics
        };

        await Assert.ThrowsAsync<System.ArgumentException>(async () => await handler.Handle(cmd, CancellationToken.None));
        repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Name_Invalid()
    {
        var repoMock = new Mock<IProductRepository>();
        var handler = new CreateProductHandler(repoMock.Object);

        var cmd = new CreateProductCommand
        {
            Name = " ",
            Description = "Valid",
            Price = 10m,
            Category = ProductCategory.Electronics
        };

        await Assert.ThrowsAsync<System.ArgumentException>(async () => await handler.Handle(cmd, CancellationToken.None));
        repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Category_Undefined()
    {
        var repoMock = new Mock<IProductRepository>();
        var handler = new CreateProductHandler(repoMock.Object);

        var cmd = new CreateProductCommand
        {
            Name = "Valid",
            Description = "Valid",
            Price = 10m,
            Category = ProductCategory.Undefined
        };

        await Assert.ThrowsAsync<System.ArgumentException>(async () => await handler.Handle(cmd, CancellationToken.None));
        repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
