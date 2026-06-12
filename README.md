# dnd-compendium-service

Bounded context responsavel pelo catalogo versionado de regras de D&D. O Compendium expoe dados canonicos, versionados e estruturados para BFF, Character Builder e Rules Engine. Ele nao cria personagens, nao calcula ficha final e nao decide se uma ficha esta pronta.

## Arquitetura

O servico segue quatro camadas:

- `src/domain/Compendium.Domain`: agregados, entidades, value objects, invariantes, specifications e eventos de dominio. Nao referencia HTTP, ORM, banco ou mensageria.
- `src/application/Compendium.Application`: use cases, commands, queries, portas, resultado de aplicacao e servicos de aplicacao.
- `src/presentation/Compendium.API`: API HTTP, DTOs, health checks, OpenAPI e mapeamento de erros para HTTP.
- `src/infrastructure/Compendium.Infra`: EF Core, PostgreSQL, migrations, repositories futuros, Outbox, Inbox e integracoes.

Suites de teste:

- `tests/unit`: dominio e application sem infraestrutura.
- `tests/integration`: persistencia, migrations e use cases com infraestrutura.
- `tests/contract`: contratos HTTP internos.

## Banco de dados

O schema do servico e `compendium`. As migrations do Compendium nao devem alterar schemas de outros servicos nem criar foreign keys fisicas para fora do schema.

Connection string local padrao:

```text
Host=localhost;Port=5432;Database=compendium;Username=compendium;Password=compendium
```

Pode ser sobrescrita por configuracao ASP.NET Core, por exemplo:

```powershell
$env:ConnectionStrings__CompendiumDb="Host=localhost;Port=5432;Database=compendium;Username=compendium;Password=compendium"
```

## Comandos

Restaurar ferramentas locais:

```powershell
dotnet tool restore
```

Restaurar pacotes:

```powershell
dotnet restore dnd-compendium-service.slnx
```

Build:

```powershell
dotnet build dnd-compendium-service.slnx
```

Testes:

```powershell
dotnet test dnd-compendium-service.slnx
```

Rodar API:

```powershell
dotnet run --project src/presentation/Compendium.API/Compendium.API.csproj
```

Health check:

```text
GET /health
GET /health/ready
```

Gerar migration:

```powershell
dotnet ef migrations add NomeDaMigration --project src/infrastructure/Compendium.Infra/Compendium.Infra.csproj --startup-project src/presentation/Compendium.API/Compendium.API.csproj --output-dir Persistence/Migrations
```

Aplicar migrations:

```powershell
dotnet ef database update --project src/infrastructure/Compendium.Infra/Compendium.Infra.csproj --startup-project src/presentation/Compendium.API/Compendium.API.csproj
```

Reverter a ultima migration local:

```powershell
dotnet ef database update 0 --project src/infrastructure/Compendium.Infra/Compendium.Infra.csproj --startup-project src/presentation/Compendium.API/Compendium.API.csproj
```

## Estrategia expand/contract

Mudancas de banco devem ser feitas em passos compativeis:

1. Expandir o schema sem quebrar consumidores existentes.
2. Publicar aplicacao que escreve e le ambos os formatos quando necessario.
3. Migrar dados.
4. Remover colunas/tabelas antigas apenas quando nao houver consumidores usando o contrato anterior.

Campos de regras de dominio, efeitos, escolhas, prerequisitos, magias, equipamentos e snapshots devem ser relacionais. Nao usar JSON para modelar regra mecanica.
