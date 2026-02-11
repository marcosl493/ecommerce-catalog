using Application.Interfaces;
using Application.UseCases.EditProduct;
using Bogus;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases;

public class EditProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Fail_When_Product_Not_Found()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new EditProductHandler(repoMock.Object);

        var cmd = new EditProductCommand(Guid.NewGuid(), "Name", "Desc", 10m, true, ProductCategory.Electronics);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_Should_Return_NoContent_When_No_Change()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new EditProductHandler(repoMock.Object);

        var cmd = new EditProductCommand(product.Id, null, null, null, null, null);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_Should_Update_Product_When_Changes()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new EditProductHandler(repoMock.Object);

        var newName = faker.Commerce.ProductName();
        var newDescription = faker.Commerce.ProductDescription();
        var newPrice = price + 10;
        var cmd = new EditProductCommand(product.Id, newName, newDescription, newPrice, false, ProductCategory.Books);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(newName, result.Value.Name);
        Assert.Equal(newDescription, result.Value.Description);
        Assert.Equal(newPrice, result.Value.Price);
        Assert.Equal(ProductCategory.Books, result.Value.Category);
        Assert.False(result.Value.Active);

        repoMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p == product), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Price_Invalid()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new EditProductHandler(repoMock.Object);

        var cmd = new EditProductCommand(product.Id, null, null, 0m, null, null);

        await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(cmd, CancellationToken.None));
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Category_Undefined()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new EditProductHandler(repoMock.Object);

        var cmd = new EditProductCommand(product.Id, null, null, null, null, ProductCategory.Undefined);

        await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(cmd, CancellationToken.None));
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
