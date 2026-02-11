using Application.Common;
using Application.Interfaces;
using Application.UseCases.GetProduct;
using Bogus;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases;

public class GetProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Single_Product_When_Id_Provided_And_Found()
    {
        var faker = new Faker();
        var product = new Product(faker.Commerce.ProductName(), faker.Commerce.ProductDescription(), faker.Random.Decimal(1, 1000), ProductCategory.Electronics);

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new GetProductHandler(repoMock.Object);

        var query = new GetProductQuery { Id = product.Id };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var paged = result.Value;
        Assert.NotNull(paged);
        var list = paged.Items.ToList();
        Assert.Single(list);
        Assert.Equal(1, paged.TotalCount);
        Assert.Equal(product, list[0]);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Id_Provided_And_Not_Found()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new GetProductHandler(repoMock.Object);

        var query = new GetProductQuery { Id = Guid.NewGuid() };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("nao foi encontrado", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_Should_Return_Paged_Result_When_No_Id()
    {
        var faker = new Faker();
        var products = new List<Product>
        {
            new Product(faker.Commerce.ProductName(), faker.Commerce.ProductDescription(), faker.Random.Decimal(1,1000), ProductCategory.Electronics),
            new Product(faker.Commerce.ProductName(), faker.Commerce.ProductDescription(), faker.Random.Decimal(1,1000), ProductCategory.Electronics)
        };

        var paged = new PagedResult<Product>
        {
            Items = products,
            TotalCount = products.Count,
            Page = 1,
            PageSize = 10
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.QueryAsync(null, null, null, null, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var handler = new GetProductHandler(repoMock.Object);

        var query = new GetProductQuery { Page = 1, PageSize = 10 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(products, result.Value.Items);
        Assert.Equal(products.Count, result.Value.TotalCount);
    }
}
