using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Services;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class MangaApiSyncService : SyncServiceBase<MangaVolume> {
    private readonly ILogger _logger;

    public MangaApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    public override async Task<List<MangaVolume>> GetAsync(DateTime since) {
        _logger.Information("Iniciando 'Loading' de Mangás para a conexão {Description}", Connection.Description);

        var apiClient = new ApiClientService(Connection, _logger);
        var mangaApiService = new MangaApiService(apiClient, _logger);

        var tables = await mangaApiService.GetTablesAsync();
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhuma tabela encontrada para a conexão {Description}", Connection.Description);
            return new List<MangaVolume>();
        }

        // Lógica para carregar os DTOs da API
        // Aqui você implementaria a paginação, etc.
        var pagedResponse = await mangaApiService.GetUpdatesAsync(tables.First(), since, 0);

        if (pagedResponse?.Content == null)
            return new List<MangaVolume>();

        // Lógica de Mapeamento de DTO para Entidade (simplificado)
        var volumes = new List<MangaVolume>();
        foreach (var dto in pagedResponse.Content) {
            // Aqui você usaria uma classe de Mapper (ex: AutoMapper) para converter DTO em Entidade
            volumes.Add(new MangaVolume { Id = dto.Id, Manga = dto.Manga, /* ... etc ... */ });
        }
        return volumes;
    }

    public override async Task SaveAsync(List<MangaVolume> entities) {
        _logger.Information("Iniciando 'Save' de {Count} volumes de Mangá", entities.Count);

        // Lógica para obter a conexão com o banco de dados de destino e usar o DAO
        // Esta parte depende de como você gerencia a conexão do seu DAO JDBC.
        // Exemplo hipotético:
        // await using (var dbConnection = new MySqlConnection("sua_connection_string_destino"))
        // {
        //     await dbConnection.OpenAsync();
        //     var mangaDao = DaoFactory.CreateMangaExtractorDao(dbConnection, "nome_do_banco_destino");
        //     foreach (var volume in entities)
        //     {
        //         await mangaDao.UpdateVolumeAsync("nome_do_banco_destino", volume);
        //     }
        // }
    }

    public override Task DeleteAsync(List<MangaVolume> entities) {
        _logger.Information("Iniciando 'Delete' de {Count} volumes de Mangá", entities.Count);
        // Lógica de exclusão aqui...
        throw new NotImplementedException();
    }
}