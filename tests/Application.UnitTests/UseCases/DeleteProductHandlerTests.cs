using Application.Interfaces;
using Application.UseCases.DeleteProduct;
using Bogus;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases;

public class DeleteProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Product_When_Found()
    {
        var faker = new Faker();
        var product = new Product(faker.Commerce.ProductName(), faker.Commerce.ProductDescription(), faker.Random.Decimal(1, 1000), ProductCategory.Electronics);

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);
        repoMock.Setup(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        var handler = new DeleteProductHandler(repoMock.Object);

        var cmd = new DeleteProductCommand(product.Id);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repoMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Product_Not_Found()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new DeleteProductHandler(repoMock.Object);

        var cmd = new DeleteProductCommand(Guid.NewGuid());

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("não foi encontrado", result.Errors[0].Message);
    }
}
