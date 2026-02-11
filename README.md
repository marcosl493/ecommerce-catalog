# ecommerce-catalog

API de catálogo de produtos (exemplo)

Descrição
---------
Projeto exemplo que implementa um catálogo de produtos com arquitetura em camadas:

- Domain: entidades e value objects do domínio (`Product`, `ProductImage`, `ProductCategory`).
- Application: casos de uso (commands/queries) e regras de negócio (handlers). Usa MediatR e FluentResults.
- Infrastructure: persistência com EF Core (Postgres) e implementação simulada de armazenamento S3 local.
- WebApi: endpoints minimal API com documentação e serialização de `FluentResults` para respostas HTTP.

Funcionalidades
--------------
- Criar produto (POST `/api/products`)
- Obter produtos e filtros (GET `/api/products`)
- Editar produto (PUT `/api/products/{id}`)
- Deletar produto (DELETE `/api/products/{id}`)
- Enviar imagem de produto (simulação S3) (POST `/api/products/{id}/image`) — aceita `multipart/form-data`

Persistência
------------
- Utiliza EF Core com provider Npgsql (Postgres).
- Migrations incluídas em `src/Infrastructure/Persistence/Migrations`.

Armazenamento de imagens
------------------------
- Interface `IStorageService` em `src/Application/Interfaces/IStorageService.cs`.
- Implementação simulada `AwsS3StorageService` em `src/Infrastructure/Storage/AwsS3StorageService.cs` escreve arquivos em `uploads/{bucket}/{key}` e retorna uma URI simulada.

Requisitos
---------
- .NET 10 SDK
- Postgres (opcional para rodar a aplicação; necessário para usar a persistência real)

Configuração
------------
As variáveis e configurações importantes:
- `ConnectionStrings:CatalogDbContext` — connection string base para Postgres
- `DB_USERNAME` e `DB_PASSWORD` — credenciais do banco
- `Storage:BaseUrl` (opcional) — base url simulada para arquivos (padrão `file://`)
- `Storage:Bucket` (opcional) — bucket default (padrão `local-bucket`)

Como executar
-------------
1. Restaurar e build

```bash
dotnet restore
dotnet build
```

2. Aplicar migrations (se usar Postgres local):

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
```

3. Executar API

```bash
dotnet run --project src/WebApi
```

4. Documentação em ambiente de desenvolvimento
- Swagger/OpenAPI e referências aparecem quando `ASPNETCORE_ENVIRONMENT=Development`.

Testes
-----
Testes unitários estão em `tests/Application.UnitTests`.

Executar testes:

```bash
dotnet test
```

Notas de implementação
----------------------
- Handlers usam `FluentResults` para sinalizar sucesso/falha e são serializados por `WebApi/ResultSerializer`.
- `UploadProductImageHandler` valida tipo (por magic bytes) e tamanho (máximo 10 MB) antes de enviar para `IStorageService`.
- `EditProductHandler` retorna `Result.Ok()` vazio quando nenhuma propriedade foi alterada, e `Result.Ok(product)` quando há alterações.

Próximos passos / melhorias
--------------------------
- Implementar integração real com AWS S3 (substituindo `AwsS3StorageService`) ou adicionar suporte a MinIO.
- Adicionar testes de integração end-to-end.
- Adicionar autenticação/autorização nos endpoints.