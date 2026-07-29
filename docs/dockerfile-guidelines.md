# Padrao de Dockerfiles dos servicos

Este documento registra as decisoes adotadas no `dnd-compendium-service` e deve ser usado como
referencia para os proximos servicos .NET.

## Imagem e plataforma

- Usar imagens oficiais da Microsoft em `mcr.microsoft.com/dotnet`.
- Usar a mesma versao principal do .NET declarada no projeto.
- Usar a variante Ubuntu `noble`.
- Construir explicitamente para `linux/amd64`.
- Usar imagem de SDK apenas no estagio de build e imagem ASP.NET Runtime no estagio final.
- Nao incluir testes no Dockerfile. O pipeline deve testa-los antes de construir/publicar a imagem.

## Build

- Manter o contexto de build na raiz do repositorio.
- Copiar primeiro os arquivos de projeto e executar `dotnet restore`, aproveitando o cache de
  camadas.
- Usar cache NuGet do BuildKit no `restore` e no `publish`.
- Publicar em `Release`, como `framework-dependent`, para `linux-x64`.
- Manter um `.dockerignore` com lista de inclusao para evitar o envio de fontes e artefatos
  desnecessarios ao daemon.

Exemplo:

```bash
docker buildx build \
  --platform linux/amd64 \
  --tag nome-do-servico:local \
  --load \
  .
```

O pipeline deve habilitar BuildKit ou usar `docker buildx`.

## Runtime e seguranca

- Executar a aplicacao como usuario nao-root.
- Usar a porta interna `8080`.
- Definir `ASPNETCORE_ENVIRONMENT=Production` como padrao.
- Nao copiar SDK, codigo-fonte, testes, configuracoes de IDE ou repositorio Git para a imagem final.
- Nao gravar connection strings, chaves de API ou outros segredos no Dockerfile.
- Fornecer segredos por variaveis protegidas do pipeline ou pelo mecanismo de secrets da
  plataforma.
- Nao usar argumentos de build para segredos, pois eles podem permanecer no historico da imagem.
- Fixar a linha principal da imagem (`10.0`) e reconstruir regularmente para incorporar patches.

## Operacao

- Declarar `HEALTHCHECK` contra o endpoint de liveness do servico.
- Usar o endpoint de readiness da aplicacao na plataforma quando ela suportar probes separados.
- Aplicar migrations manualmente, fora do processo de inicializacao da API.
- Entregar a connection string por `ConnectionStrings__NomeDaConexao`.
- Publicar a porta do host somente na execucao; ela nao deve ser fixada no Dockerfile.

## Pipeline recomendado

A ordem esperada e:

1. Restaurar dependencias.
2. Compilar.
3. Executar testes e validacoes de cobertura.
4. Construir a imagem para `linux/amd64`.
5. Analisar a imagem com o scanner de seguranca adotado pela equipe.
6. Publicar a imagem no registry com uma tag imutavel, como o SHA do commit.
7. Aplicar migrations por uma etapa manual/controlada.
8. Implantar a imagem passando configuracoes e segredos em runtime.
9. Confirmar os endpoints `/health` e `/health/ready`.

Tags de ambiente, como `staging` ou `latest`, podem ser adicionais, mas nao devem substituir a tag
imutavel usada para rollback e auditoria.
