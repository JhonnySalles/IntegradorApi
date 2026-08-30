# Módulo Novel Extractor - NovelExtractor

## 🎯 Objetivo / Contexto

O módulo `NovelExtractor` é responsável pela estrutura e sincronização de *Light Novels* e *Web Novels*, gerenciando dados de volumes, capítulos, textos traduzidos/brutos, capas e vocabulário extraído. Permite a sincronização dinâmica por tabelas no banco de dados MySQL e via endpoints REST.

## 🧩 Arquivos e Componentes

- **Serviço de API (`IntegradorApi.Api/Services/NovelApiService.cs`)**:
  - Encapsula as chamadas HTTP para o backend REST (`/api/novel-extractor`).
- **Serviços de Sincronização (`IntegradorApi.Sync`)**:
  - `NovelApiSyncService.cs`: Sincronizador para origens/destinos do tipo `APIREST`.
  - `NovelDataSyncService.cs`: Sincronizador para origens/destinos do tipo `MYSQL`.
- **Serviço de Dados (`IntegradorApi.Data/Service/NovelDataService.cs`)**:
  - Realiza operações diretas no banco de dados MySQL via DAOs/JDBC.
- **DAO e Repositório (`IntegradorApi.Data/Repositories/Interfaces/INovelExtractorDao.cs`)**:
  - Interface DAO para operações em tabelas de novel.
- **Entidades de Banco (`IntegradorApi.Data/Models/NovelExtractor/`)**:
  - `NovelVolume.cs`: Representa o volume da novel (título, autor, série, idioma).
  - `NovelCapitulo.cs`: Representa os capítulos do volume.
  - `NovelTexto.cs`: Representa o conteúdo de texto dos capítulos.
  - `NovelCapa.cs`: Capa e metadados de imagem.
  - `NovelVocabulario.cs`: Vocabulário e glossário extraído do texto.
- **DTOs (`IntegradorApi.Api/Models/NovelDtos.cs`)**:
  - `NovelVolumeDto`, `NovelCapituloDto`, `NovelTextoDto`.

## ⚙️ Regras de Negócio e Estrutura de Tabelas

- **Tabelas Dinâmicas**: Assim como no módulo de mangás, as novels são segregadas em tabelas dinâmicas no banco (ex.: `novel_volumes_01`). O integrador primeiro consulta as tabelas ativas (`GetTablesAsync`) e processa cada tabela separadamente.
- **Estrutura de Conteúdo**: Um volume (`NovelVolume`) agrupa uma coleção de capítulos e dados de capa. O conteúdo textual é associado aos capítulos para facilitar o envio em lotes paginados.

## 🔄 Endpoints REST / Consultas HTTP

- **Listar Tabelas Disponíveis**:
  - `GET /api/novel-extractor/tabelas`
  - Retorna uma lista de strings com os nomes das tabelas de novels.
- **Buscar Atualizações Incrementais**:
  - `GET /api/novel-extractor/tabela/{tableName}/atualizacao/{formattedDate}?page=0&size=20&direction=asc`
  - Retorna `PagedApiResponse<NovelVolumeDto>`.
- **Enviar Atualizações (Salvar/Patch)**:
  - `PATCH /api/novel-extractor/tabela/{tableName}/lista`
  - Envia uma `List<NovelVolumeDto>` no corpo da requisição.
- **Remover Registros (Delete)**:
  - `DELETE /api/novel-extractor/tabela/{tableName}/lista`
  - Envia uma `List<NovelVolumeDto>` no corpo da requisição para deleção remota.
