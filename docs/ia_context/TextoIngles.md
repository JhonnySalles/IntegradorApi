# Módulo Texto Inglês - TextoIngles

## 🎯 Objetivo / Contexto

O módulo `TextoIngles` gerencia a infraestrutura de estudo e análise linguística para o idioma inglês. Ele contempla repositórios de vocabulário, palavras validadas, listas de palavras excluídas do estudo e filas de palavras pendentes de revisão.

## 🧩 Arquivos e Componentes

- **Serviço API (`IntegradorApi.Api/Services/TextoInglesApiService.cs`)**:
  - Implementa chamadas HTTP REST organizadas por sub-módulos.
- **Serviço de Dados (`IntegradorApi.Data/Service/TextoInglesDataService.cs`)**:
  - Acessa o banco MySQL utilizando o `TextoInglesDbContext`.
- **DbContext (`IntegradorApi.Data/Data/TextoInglesDbContext.cs`)**:
  - Mapeia as 4 tabelas principais do esquema de Texto Inglês.
- **Entidades (`IntegradorApi.Data/Models/TextoIngles/`)**:
  - `ExclusaoIngles.cs`: Palavras ignoradas/excluídas do estudo.
  - `RevisarIngles.cs`: Palavras ou frases que requerem revisão manual.
  - `ValidoIngles.cs`: Termos validados e aprovados.
  - `VocabularioIngles.cs`: Repositório geral de vocabulário em inglês com frequência e definições.
- **DTOs (`IntegradorApi.Api/Models/TextoInglesDto.cs`)**:
  - DTOs correspondentes (`ExclusaoDto`, `RevisarDto`, `ValidoDto`, `VocabularioDto`).

## ⚙️ Estrutura de Sub-módulos e Endpoints REST

O módulo é subdividido em 4 sub-módulos independentes:

### 1. Sub-módulo Exclusão (`exclusao`)
- `GET /api/texto-ingles/exclusao/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-ingles/exclusao/lista`
- `DELETE /api/texto-ingles/exclusao/lista`

### 2. Sub-módulo Revisar (`revisar`)
- `GET /api/texto-ingles/revisar/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-ingles/revisar/lista`
- `DELETE /api/texto-ingles/revisar/lista`

### 3. Sub-módulo Válido (`valido`)
- `GET /api/texto-ingles/valido/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-ingles/valido/lista`
- `DELETE /api/texto-ingles/valido/lista`

### 4. Sub-módulo Vocabulário (`vocabulario`)
- `GET /api/texto-ingles/vocabulario/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-ingles/vocabulario/lista`
- `DELETE /api/texto-ingles/vocabulario/lista`
