# Versionamento dos contratos HTTP internos

Os DTOs publicos internos usam sufixo `V1` e carregam `apiVersion: "v1"` no corpo quando a
resposta representa uma projecao composta. Consumidores dependem desses contratos, nunca de
entidades de dominio ou modelos do EF Core.

Mudancas aditivas opcionais podem permanecer na versao atual. Renomear ou remover campos, alterar
tipo, semantica, nulabilidade ou cardinalidade exige um novo DTO e uma nova versao de rota ou
media type. A versao anterior continua coberta por testes de contrato durante a janela de migracao
acordada com BFF, Character Builder e Rules Engine.

Os contratos cobertos no estado atual incluem fontes, classes, opcoes de criacao, detalhes
mecanicos, feed de mudancas e traducoes. Contratos de magias, especies, backgrounds e feats serao
adicionados quando esses agregados existirem; colecoes ainda nao implementadas permanecem vazias
em `v1`, sem payload JSON arbitrario.
