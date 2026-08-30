# Módulo Texto Japonês - TextoJapones

## 🎯 Objetivo / Contexto

O módulo `TextoJapones` é o repositório de análise linguística para a língua japonesa. Ele suporta o processamento de Kanji (leituras Kun'yomi/On'yomi, radicais, traços e nível JLPT), dicionário Kanjax traduzido em português, estatísticas de ocorrência, filas de revisão e listas de exclusão.

## 🧩 Arquivos e Componentes

- **Serviço API (`IntegradorApi.Api/Services/TextoJaponesApiService.cs`)**:
  - Encapsula as requisições HTTP REST divididas por sub-módulos.
- **Serviço de Dados (`IntegradorApi.Data/Service/TextoJaponesDataService.cs`)**:
  - Acessa o MySQL através do `TextoJaponesDbContext`.
- **DbContext (`IntegradorApi.Data/Data/TextoJaponesDbContext.cs`)**:
  - Mapeia as tabelas do esquema de Texto Japonês.
- **Entidades (`IntegradorApi.Data/Models/TextoJapones/`)**:
  - `EstatisticaJapones.cs`: Estatísticas de uso, frequência e nível de dificuldade de vocabulários e Kanjis.
  - `ExclusaoJapones.cs`: Termos ignorados na análise.
  - `FilaSqlJapones.cs`: Fila de comandos SQL assíncronos para processamento batch.
  - `KanjaxPt.cs`: Base de significados de Kanjis e ideogramas traduzidos para o Português.
  - `KanjiInfo.cs`: Metadados completos de Kanjis (radicais, número de traços, leituras Onyomi e Kunyomi, exemplos).
  - `RevisarJapones.cs`: Termos pendentes de validação.
  - `VocabularioJapones.cs`: Repositório central de vocabulário japonês.
- **DTOs (`IntegradorApi.Api/Models/TextoJaponesDto.cs`)**:
  - DTOs correspondentes (`EstatisticaDto`, `ExclusaoDto`, `KanjaxPtDto`, `KanjiInfoDto`, `RevisarDto`, `VocabularioDto`).

## ⚙️ Estrutura de Sub-módulos e Endpoints REST

O módulo é composto por 6 sub-módulos independentes:

### 1. Sub-módulo Estatística (`estatistica`)
- `GET /api/texto-japones/estatistica/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/estatistica/lista`
- `DELETE /api/texto-japones/estatistica/lista`

### 2. Sub-módulo Exclusão (`exclusao`)
- `GET /api/texto-japones/exclusao/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/exclusao/lista`
- `DELETE /api/texto-japones/exclusao/lista`

### 3. Sub-módulo Kanjax PT (`kanjax-pt`)
- `GET /api/texto-japones/kanjax-pt/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/kanjax-pt/lista`
- `DELETE /api/texto-japones/kanjax-pt/lista`

### 4. Sub-módulo Kanji Info (`kanji-info`)
- `GET /api/texto-japones/kanji-info/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/kanji-info/lista`
- `DELETE /api/texto-japones/kanji-info/lista`

### 5. Sub-módulo Revisar (`revisar`)
- `GET /api/texto-japones/revisar/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/revisar/lista`
- `DELETE /api/texto-japones/revisar/lista`

### 6. Sub-módulo Vocabulário (`vocabulario`)
- `GET /api/texto-japones/vocabulario/atualizacao/{lastUpdate}?page=0&size=20&direction=asc`
- `PATCH /api/texto-japones/vocabulario/lista`
- `DELETE /api/texto-japones/vocabulario/lista`
