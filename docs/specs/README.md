# Specifications

This directory contains both normative, implementation-driving specifications
and retrospective specifications for delivered repository changes.

The retrospective catalog documents every commit in repository history through
`6390af1`. Each retrospective status is `Done`, and each document describes the
change visible in the referenced Git diff rather than proposing future work.

## Conventions

- `MUST` and `MUST NOT` describe the delivered contract.
- Acceptance criteria describe the observable result of the historical change.
- Evidence names the principal files or tests changed by that commit; the Git
  commit remains the authoritative full change set.
- Sequence numbers follow `git log --reverse` and do not replace commit hashes.

## Feature areas

| Folder | Scope |
| --- | --- |
| `api-contracts` | HTTP contract baselines and OpenAPI documentation |
| `api-platform` | Shared MVC pipeline and operational route infrastructure |
| `architecture` | Composition, dependency boundaries, adapters, and architecture guidance |
| `classes` | Character class and subclass behavior and API adapters |
| `deployment` | Container packaging and operational delivery guidance |
| `documentation` | Repository-wide contributor and service documentation |
| `equipment` | Equipment domain, application, persistence, and API behavior |
| `features` | Reusable features, mechanical effects, choices, and API behavior |
| `fundamentals` | Fundamental rules data and ability-score methods |
| `importing` | Source-version import, validation, and API adapters |
| `internal-queries` | Internal read models and their API adapters |
| `messaging/outbox` | Integration messaging, Outbox delivery, telemetry, retention, and claims |
| `observability` | Metrics, tracing, coverage, and quality gates |
| `origins` | Species, backgrounds, feats, and their withdrawal |
| `platform` | Initial runnable service and layered solution bootstrap |
| `repository` | Repository initialization and source-control baseline |
| `security` | Authentication and authorization behavior |
| `sources` | Rule sources, rulesets, source versions, and API adapters |
| `translations` | Localization behavior and API adapters |

## Normative specifications

- [Development specification process](documentation/specification-process.md)
- [Outbox performance specification suite](messaging/outbox/README.md)
- [Outbox performance implementation plan](messaging/outbox/implementation-plan.md)

## Retrospective commit catalog

| # | Commit | Specification |
| ---: | --- | --- |
| 001 | `2726e1e` | [Initialize repository](repository/001-initialize-repository.md) |
| 002 | `62c38e4` | [Bootstrap service architecture](platform/002-bootstrap-service-architecture.md) |
| 003 | `b21dc54` | [Rule source catalog](sources/003-rule-source-catalog.md) |
| 004 | `5597ffe` | [Fundamental rule data](fundamentals/004-fundamental-rule-data.md) |
| 005 | `b806b32` | [Ability score methods](fundamentals/005-ability-score-methods.md) |
| 006 | `49b8b5c` | [Class and subclass catalog](classes/006-class-and-subclass-catalog.md) |
| 007 | `bd6e6c8` | [Features, effects, prerequisites, and choices](features/007-feature-mechanics.md) |
| 008 | `701d819` | [Species, backgrounds, and feats](origins/008-origin-catalog.md) |
| 009 | `0a4beb9` | [Withdraw origins slice](origins/009-withdraw-origin-slice.md) |
| 010 | `63f5f4d` | [Equipment catalog](equipment/010-equipment-catalog.md) |
| 011 | `dc497f9` | [Translations and migration corrections](translations/011-translations-and-migration-corrections.md) |
| 012 | `5695658` | [SRD source-version import](importing/012-srd-source-version-import.md) |
| 013 | `18410f3` | [Internal compendium queries](internal-queries/013-internal-compendium-queries.md) |
| 014 | `c5e34d4` | [Integration events and transactional messaging](messaging/outbox/014-transactional-messaging.md) |
| 015 | `5b48ec5` | [Observability and quality gates](observability/015-observability-and-quality-gates.md) |
| 016 | `69c01e9` | [Security and authorization](security/016-security-and-authorization.md) |
| 017 | `0bca199` | [HTTP contract matrix](api-contracts/017-http-contract-matrix.md) |
| 018 | `31edeb6` | [Containerization and operator documentation](deployment/018-containerization-and-docs.md) |
| 019 | `c4c0872` | [Cross-cutting composition project](architecture/019-cross-cutting-project.md) |
| 020 | `0dd0d98` | [Centralized dependency registration](architecture/020-centralized-dependency-registration.md) |
| 021 | `8100eeb` | [MVC front-controller pipeline](api-platform/021-mvc-front-controller-pipeline.md) |
| 022 | `f3d6425` | [Source MVC controllers](sources/022-source-mvc-controllers.md) |
| 023 | `dda744f` | [Fundamental MVC controllers](fundamentals/023-fundamental-mvc-controllers.md) |
| 024 | `2e75ee8` | [Class MVC controllers](classes/024-class-mvc-controllers.md) |
| 025 | `4551822` | [Feature MVC controllers](features/025-feature-mvc-controllers.md) |
| 026 | `d0ffe79` | [Equipment MVC controllers](equipment/026-equipment-mvc-controllers.md) |
| 027 | `6f05868` | [Translation MVC controller](translations/027-translation-mvc-controller.md) |
| 028 | `4261668` | [Import MVC controller](importing/028-import-mvc-controller.md) |
| 029 | `513ead8` | [Internal query MVC controllers](internal-queries/029-internal-query-mvc-controllers.md) |
| 030 | `68ff48f` | [Operational MVC controllers](api-platform/030-operational-mvc-controllers.md) |
| 031 | `7fe24f2` | [Remove legacy endpoint infrastructure](api-platform/031-remove-legacy-endpoint-infrastructure.md) |
| 032 | `4837e74` | [DDD architecture tests](architecture/032-ddd-architecture-tests.md) |
| 033 | `f8da5f5` | [Readable equipment application layer](equipment/033-readable-equipment-application-layer.md) |
| 034 | `188b692` | [Readable persistence adapters](architecture/034-readable-persistence-adapters.md) |
| 035 | `e7145ec` | [MVC ports-and-adapters documentation](architecture/035-architecture-documentation.md) |
| 036 | `a0959cc` | [Structured service README](documentation/036-structured-service-readme.md) |
| 037 | `458809a` | [Swagger descriptions and examples](api-contracts/037-swagger-documentation.md) |
| 038 | `4606747` | [Outbox performance specification suite](messaging/outbox/038-outbox-performance-specifications.md) |
| 039 | `8154fcd` | [Decoupled Outbox backlog telemetry](messaging/outbox/039-decoupled-backlog-telemetry.md) |
| 040 | `175f7d4` | [Indexed Outbox retention cleanup](messaging/outbox/040-indexed-outbox-retention.md) |
| 041 | `0c3585e` | [Lease-based Outbox claiming](messaging/outbox/041-lease-based-outbox-claiming.md) |
| 042 | `6390af1` | [Backlog metric alias contract coverage](messaging/outbox/042-backlog-metric-alias-tests.md) |
