# Visão Geral da Arquitetura - IntegradorApi

## 🎯 Objetivo / Contexto

O `IntegradorApi` é uma aplicação desktop desenvolvida em C# .NET 8 (WinUI 3 / Windows App SDK) responsável por orquestrar, sincronizar e transferir dados de maneira incremental entre bancos de dados locais (MySQL) e APIs REST remotas ou entre instâncias de bancos distribuídos. O sistema realiza busca incremental orientada por data de alteração (`lastSyncDate`), suporta deleção opcional na origem, gerenciamento de autenticação via JWT Token Bearer e execução agendada em segundo plano com suporte a minimização para a bandeja do sistema (System Tray).

## 🧩 Estrutura de Diretórios e Padrão da Solução

A solução Visual Studio é composta por 4 projetos interdependentes:

- **`IntegradorApi` (Desktop Client WinUI 3)**:
  - `IntegradorApi.xaml` / `IntegradorApi.xaml.cs`: Interface principal WinUI 3 com abas de Sincronização e Conexões, console de logs, controles de status e menu tray (`H.NotifyIcon`).
  - `AppConfig.cs` / `SettingsService.cs`: Gerenciador de configurações locais e estado de sincronização persistido no `appsettings.json`.
  - `ViewModels/`: ViewModels para controle visual de status e binding de conexões.
- **`IntegradorApi.Api` (Camada HTTP Client REST)**:
  - `Services/ApiClientService.cs`: Cliente HTTP genérico (`HttpClient`) com autenticação automática via JWT (`/auth/signin`), renovação de token, injeção do cabeçalho `Authorization: Bearer <token>` e tratamento dos verbos GET, PATCH e DELETE.
  - `Core/ApiAuthManager.cs`: Gerenciador estático de estado do token de acesso, refresh token e expiração.
  - `Services/`: Serviços de API especializados por módulo (`MangaApiService`, `NovelApiService`, `DeckSubtitleApiService`, `ComicInfoApiService`, `TextoInglesApiService`, `TextoJaponesApiService`).
  - `Models/`: DTOs e estruturas de resposta paginada (`PagedApiResponse<T>`).
- **`IntegradorApi.Data` (Camada de Persistência e Acesso a Dados)**:
  - `Data/AppDbContext.cs`: Contexto EF Core para MySQL para armazenar conexões configuradas e histórico de sincronizações (`Connection`, `Sincronization`).
  - `Data/TextoInglesDbContext.cs` & `TextoJaponesDbContext.cs`: DbContexts dedicados para tabelas dos esquemas Texto Inglês e Texto Japonês.
  - `Repositories/`: Interfaces e DAOs em JDBC/MySQL (`IMangaExtractorDao`, `INovelExtractorDao`, `IDeckSubtitleDao`, `IComicInfoDao`, `DaoFactory`).
  - `Service/DatabaseService.cs`: Gerenciador de migrações e operações CRUD de conexões no banco local.
  - `Models/`: Entidades do domínio separadas por esquemas/subpastas.
  - `Enums/`: Enumerações de `ConnectionType`, `DataSourceType`, `IntegrationType` e `Linguagens`.
- **`IntegradorApi.Sync` (Motor de Orquestração de Sincronização)**:
  - `Services/SyncOrchestrator.cs`: Orquestrador central de sincronização que lê originais e destinos ativos e executa a transferência por página.
  - `Services/SyncServiceBase.cs`, `SyncApiServiceBase.cs`, `SyncDataServiceBase.cs`: Classes abstratas base para implementação de leituras e gravações genéricas.
  - `Mappings/`: Mapeamentos de DTO para Entidade usando `AutoMapper`.

## ⚙️ Tecnologias e Dependências Core

- **Linguagem & Runtime:** C# 12 / .NET 8.0 SDK
- **Framework UI:** WinUI 3 (Windows App SDK 1.5+)
- **ORM & Banco Local:** Entity Framework Core 8.0 + Pomelo.EntityFrameworkCore.MySql 8.0
- **Conector Banco Relacional:** MySqlConnector
- **Comunicação HTTP:** `System.Net.Http.HttpClient` + Newtonsoft.Json
- **Mapeamento de Objetos:** AutoMapper 13.0
- **Logging & Diagnostic:** Serilog 3.1 (Sinks para File e Event Bus interno)
- **Bandeja do Sistema:** H.NotifyIcon.WinUI 2.0
- **Componentes UI adicionais:** CommunityToolkit.WinUI.UI.Controls

## 🔄 Fluxo de Processamento Padrão

1. **Leitura de Conexões**: Ao iniciar ou disparar a sincronização, o `SyncOrchestrator` consulta as conexões ativas no `AppDbContext`.
2. **Pareamento Origem x Destino**: O orquestrador agrupa conexões do tipo `ORIGIN` e localiza conexões do tipo `DESTINATION` com o mesmo `IntegrationType`.
3. **Autenticação (Se REST API)**: Caso a conexão seja do tipo `APIREST`, o `ApiClientService` faz login automático em `/auth/signin` utilizando usuário e senha cadastrados.
4. **Consulta Incremental**: A origem é consultada passando a última data de sincronização (`lastSyncDate`). Apenas registros alterados/criados após essa data são retornados.
5. **Transferência em Lotes (Batch/Paginado)**: Os registros retornados são processados por página (tamanho padrão: 20 registros) e enviados para cada destino ativo (`SaveAsync` / `PATCH`).
6. **Remoção Opcional**: Se a flag `Delete` estiver habilitada na conexão de origem, o integrador executa `DeleteAsync` na origem para limpar os registros transferidos.
7. **Atualização do Timestamp**: A data/hora da sincronização bem-sucedida é gravada na tabela `Sincronization` do banco local.
