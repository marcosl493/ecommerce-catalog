using Application.Interfaces;
using Application.UseCases.UploadProductImage;
using Bogus;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases;

public class UploadProductImageHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Product_On_Successful_Upload()
    {
        // Arrange
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var storageMock = new Mock<IStorageService>();
        var expectedUri = new Uri("https://s3.local/products/somekey.jpg");
        storageMock.Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedUri);

        var handler = new UploadProductImageHandler(repoMock.Object, storageMock.Object);

        var content = new byte[] { 0xFF, 0xD8, 0xFF }.Concat(faker.Random.Bytes(100)).ToArray(); // JPEG magic bytes + random content
        var fileName = faker.System.FileName("jpg");
        var cmd = new UploadProductImageCommand(product.Id, content, fileName);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedUri.AbsoluteUri, result.Value.Image?.Path);
        repoMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p == product), It.IsAny<CancellationToken>()), Times.Once);
        storageMock.Verify(s => s.UploadAsync("products", It.IsAny<string>(), content, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Product_Not_Found()
    {
        var faker = new Faker();
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

        var storageMock = new Mock<IStorageService>();

        var handler = new UploadProductImageHandler(repoMock.Object, storageMock.Object);

        var cmd = new UploadProductImageCommand(Guid.NewGuid(), faker.Random.Bytes(10), faker.System.FileName("jpg"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("nao foi encontrado", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_Should_Reject_Empty_Content()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

        var storageMock = new Mock<IStorageService>();

        var handler = new UploadProductImageHandler(repoMock.Object, storageMock.Object);

        var cmd = new UploadProductImageCommand(product.Id, Array.Empty<byte>(), faker.System.FileName("jpg"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("Arquivo invalido ou vazio", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_Should_Reject_Too_Large_File()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

        var storageMock = new Mock<IStorageService>();

        var handler = new UploadProductImageHandler(repoMock.Object, storageMock.Object);

        var tooLarge = faker.Random.Bytes(10 * 1024 * 1024 + 1); // 10MB + 1
        var cmd = new UploadProductImageCommand(product.Id, tooLarge, faker.System.FileName("jpg"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("no maximo 10 MB", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_Should_Reject_Non_Image_File()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var price = faker.Random.Decimal(1, 1000);
        var category = ProductCategory.Electronics;

        var product = new Product(name, description, price, category);
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

        var storageMock = new Mock<IStorageService>();

        var handler = new UploadProductImageHandler(repoMock.Object, storageMock.Object);

        var notImage = faker.Random.Bytes(10);
        // ensure not image: set first bytes to non-image signature
        notImage[0] = 0x00;
        notImage[1] = 0x01;
        notImage[2] = 0x02;
        var cmd = new UploadProductImageCommand(product.Id, notImage, faker.System.FileName("bin"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("deve ser uma imagem valida", result.Errors[0].Message);
    }
}
