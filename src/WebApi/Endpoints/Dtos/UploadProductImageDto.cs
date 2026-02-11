using Microsoft.AspNetCore.Http;

namespace WebApi.Endpoints.Dtos;

public sealed record UploadProductImageDto(IFormFile File);
