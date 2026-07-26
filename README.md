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

Testes com cobertura da camada de dominio:

```powershell
dotnet test dnd-compendium-service.slnx --settings coverlet.runsettings --collect:"XPlat Code Coverage"
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

## Importacao controlada de uma versao de fonte

O EPIC 11 expoe um fluxo administrativo transacional e idempotente:

```text
POST /api/compendium/source-versions/{sourceVersionId}/imports
POST /api/compendium/source-versions/{sourceVersionId}/validation
GET  /api/compendium/source-versions/{sourceVersionId}/validation/issues
```

O corpo da importacao e o manifesto de seed, com colecoes tipadas `abilities`, `skills`, `languages`,
`proficiencies`, `hitDice` e `equipment`. O importador valida ruleset, fonte, versao, value objects e
referencias antes de persistir. A mesma versao importada novamente retorna o registro anterior sem
duplicar entidades ou eventos.

Cada importacao bem-sucedida grava `source_version_imports` e
`compendium.source-version-imported.v1` na Outbox dentro da mesma transacao. A validacao persiste
issues `BLOCKER`, `WARNING` e `INFO`; a versao so recebe status `Imported` quando nao existem
blockers. Categorias ainda nao modeladas no servico (species, backgrounds, feats e spells) aparecem
como issues claras, em vez de serem armazenadas em JSON ou ignoradas silenciosamente.

## APIs internas de consulta

O EPIC 12 disponibiliza contratos de leitura `v1` para BFF, Character Builder e Rules Engine:

```text
GET /internal/compendium/character-creation-options?ruleset_id={id}&source_version_id={id}&locale=pt-BR&level=1
GET /internal/compendium/entities/{entityType}/{entityId}/mechanics?locale=pt-BR
GET /internal/compendium/changes?source_version_id={id}&entity_type=feature&revision=0&page=1&page_size=50
```

A consulta de opcoes retorna classes, metodos de atributo, proficiencias, idiomas e equipamentos
da versao solicitada, aplicando traducoes por locale quando existentes. Species, backgrounds e
spell lists permanecem como colecoes vazias enquanto esses agregados nao estiverem modelados.

Os detalhes mecanicos suportam `class`, `feature`, `equipment` e `choice_set`, com effects,
conditions, prerequisites e choice sets representados por campos relacionais tipados. O feed de
mudancas usa revisao crescente, filtros e paginacao. Criacoes, alteracoes e exclusoes dos agregados
acompanhados gravam `compendium_changes` e `compendium.entity-updated.v1` na Outbox na mesma
transacao. Todas as respostas internas incluem `X-Correlation-ID`.

## Eventos, Outbox e Inbox

O EPIC 13 implementa entrega assincrona *at-least-once*. Os eventos
`compendium.source-version-imported.v1`, `compendium.entity-updated.v1` e
`compendium.translation-updated.v1` sao gravados na mesma transacao dos dados de negocio.
Um servico em segundo plano publica somente registros ja commitados, registra o correlation id e,
em falhas, mantem a mensagem para retry ate move-la para `DEAD_LETTER`.

O transporte local padrao escreve a entrega no log. Em producao, `IEventTransport` deve ser
substituido pelo adapter do broker adotado sem alterar o Outbox ou os contratos em
`Compendium.Application/Contracts/Events`.

Consumidores usam `IMessageConsumer`, que registra cada combinacao de `event_id` e
`consumer_name` no Inbox. Entregas ja processadas sao ignoradas; falhas ficam disponiveis para
reprocessamento controlado e tambem terminam em `DEAD_LETTER` apos o limite configurado em
`IntegrationMessaging`.

## Qualidade e observabilidade

O pipeline em `.github/workflows/quality.yml` compila a solucao e executa as suites unitarias,
de integracao e de contrato com PostgreSQL. O limite minimo de cobertura e 50% e considera apenas `Compendium.Domain`,
mantendo os testes de dominio independentes de HTTP, ORM e banco.

Requests propagam `X-Correlation-ID`, registram `TraceId` e latencia estruturada. OpenTelemetry
exporta em `GET /metrics` duracao de endpoints e queries, mensagens pendentes e falhas da Outbox,
alem de falhas de importacao. A politica de compatibilidade dos DTOs internos esta em
`docs/http-contract-versioning.md`.
