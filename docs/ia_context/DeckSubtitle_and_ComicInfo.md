# Módulos Deck Subtitle e Comic Info - DeckSubtitle_and_ComicInfo

## 🎯 Objetivo / Contexto

Este documento detalha dois módulos de apoio da solução:
1. **Deck Subtitle (`DECKSUBTITLE`)**: Gerencia legendas, frases de estudo, tempos de sincronização de áudio/vídeo e baralhos para sistemas de memorização em espaço (Anki/SRS).
2. **Comic Info (`COMICINFO`)**: Gerencia metadados de quadrinhos e e-books no padrão *ComicInfo.xml* (título, editora, roteirista, desenhista, ano, resumo, contagem de páginas).

## 🧩 Arquivos e Componentes

### Módulo Deck Subtitle
- **Serviço API (`IntegradorApi.Api/Services/DeckSubtitleApiService.cs`)**
- **Serviços de Sync (`IntegradorApi.Sync/Services/Api/DeckSubtitleApiSyncService.cs`, `Data/DeckSubtitleDataSyncService.cs`)**
- **Serviço de Dados (`IntegradorApi.Data/Service/DeckSubtitleDataService.cs`)**
- **DAO (`IntegradorApi.Data/Repositories/Interfaces/IDeckSubtitleDao.cs`)**
- **Entidades e DTOs (`IntegradorApi.Data/Models/DeckSubtitle/Subtitle.cs`, `IntegradorApi.Api/Models/SubtitleDto.cs`)**

### Módulo Comic Info
- **Serviço API (`IntegradorApi.Api/Services/ComicInfoApiService.cs`)**
- **Serviços de Sync (`IntegradorApi.Sync/Services/Api/ComicInfoApiSyncService.cs`, `Data/ComicInfoDataSyncService.cs`)**
- **Serviço de Dados (`IntegradorApi.Data/Service/ComicInfoDataService.cs`)**
- **DAO (`IntegradorApi.Data/Repositories/Interfaces/IComicInfoDao.cs`)**
- **Entidades e DTOs (`IntegradorApi.Api/Models/ComicInfoDtos.cs`)**

## ⚙️ Regras de Negócio e Endpoints REST

### 1. Deck Subtitle (`DECKSUBTITLE`)
- **Tabelas Dinâmicas**: Trabalha com tabelas dinâmicas por baralho/legenda.
- **Endpoints**:
  - `GET /api/deck-subtitle/tabelas`: Consulta as tabelas disponíveis.
  - `GET /api/deck-subtitle/tabela/{tableName}/atualizacao/{formattedDate}?page=0&size=20&direction=asc`: Busca atualizações paginadas (`SubtitleDto`).
  - `PATCH /api/deck-subtitle/tabela/{tableName}/lista`: Envia inserções/atualizações em lote.
  - `DELETE /api/deck-subtitle/tabela/{tableName}/lista`: Envia exclusões em lote.

### 2. Comic Info (`COMICINFO`)
- **Tabela Única/Global**: Não utiliza tabelas dinâmicas, operando em um repositório centralizado de metadados de quadrinhos.
- **Endpoints**:
  - `GET /api/comic-info/atualizacao/{formattedDate}?page=0&size=20&direction=asc`: Busca atualizações paginadas (`ComicInfoDto`).
  - `PATCH /api/comic-info/lista`: Envia inserções/atualizações em lote.
  - `DELETE /api/comic-info/lista`: Envia exclusões em lote.
