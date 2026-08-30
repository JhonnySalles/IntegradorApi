# Módulo Manga Extractor - MangaExtractor

## 🎯 Objetivo / Contexto

O módulo `MangaExtractor` é responsável pela estrutura e sincronização dos metadados de mangás, incluindo volumes, capítulos, páginas, blocos de texto com coordenadas de OCR/caixa delimitadora, capas e vocabulário extraído. Ele permite a integração dinâmica por tabelas no banco de dados e via endpoints REST.

## 🧩 Arquivos e Componentes

- **Serviço de API (`IntegradorApi.Api/Services/MangaApiService.cs`)**:
  - Encapsula as chamadas HTTP para o backend REST (`/api/manga-extractor`).
- **Serviços de Sincronização (`IntegradorApi.Sync`)**:
  - `MangaApiSyncService.cs`: Sincronizador para origens/destinos do tipo `APIREST`.
  - `MangaDataSyncService.cs`: Sincronizador para origens/destinos do tipo `MYSQL`.
- **Serviço de Dados (`IntegradorApi.Data/Service/MangaDataService.cs`)**:
  - Executa consultas SQL diretas no MySQL via JDBC/DAOs.
- **DAO e Repositório (`IntegradorApi.Data/Repositories/Interfaces/IMangaExtractorDao.cs`)**:
  - Interface DAO para operações em tabelas de mangá.
- **Entidades de Banco (`IntegradorApi.Data/Models/MangaExtractor/`)**:
  - `MangaVolume.cs`: Representa o volume do mangá.
  - `MangaCapitulo.cs`: Representa os capítulos vinculados ao volume.
  - `MangaPagina.cs`: Representa as páginas do capítulo.
  - `MangaTexto.cs`: Representa os blocos de texto extraídos via OCR nas páginas.
  - `MangaCapa.cs`: Representa a capa e metadados de imagem do volume.
  - `MangaVocabulario.cs`: Vocabulário de palavras e leituras associadas ao mangá.
- **DTOs (`IntegradorApi.Api/Models/MangaDtos.cs`)**:
  - `MangaVolumeDto`, `MangaCapituloDto`, `MangaPaginaDto`, `MangaTextoDto`.

## ⚙️ Regras de Negócio e Estrutura de Tabelas

- **Tabelas Dinâmicas**: Cada mangá ou lote de mangás pode estar armazenado em uma tabela MySQL própria (ex.: `manga_volumes_01`, `manga_volumes_02`). O integrador primeiro consulta as tabelas existentes (`GetTablesAsync`) e realiza o ciclo de sincronização para cada tabela individualmente.
- **Hierarquia de Objetos**: Um volume (`MangaVolume`) contém uma lista de capítulos (`Capitulos`), que por sua vez contêm páginas (`Paginas`) e textos extraídos (`Textos`).
- **Mapeamento de Data de Alteração**: A consulta de atualização filtra por `lastUpdate` comparando com a coluna de data de modificação da tabela local/remota.

## 🔄 Endpoints REST / Consultas HTTP

- **Listar Tabelas Disponíveis**:
  - `GET /api/manga-extractor/tabelas`
  - Retorna uma lista de strings com os nomes das tabelas.
- **Buscar Atualizações Incrementeis**:
  - `GET /api/manga-extractor/tabela/{tableName}/atualizacao/{formattedDate}?page=0&size=20&direction=asc`
  - Retorna `PagedApiResponse<MangaVolumeDto>`.
- **Enviar Atualizações (Salvar/Patch)**:
  - `PATCH /api/manga-extractor/tabela/{tableName}/lista`
  - Envia uma `List<MangaVolumeDto>` no corpo da requisição.
- **Remover Registros (Delete)**:
  - `DELETE /api/manga-extractor/tabela/{tableName}/lista`
  - Envia uma `List<MangaVolumeDto>` no corpo da requisição para deleção remota.
