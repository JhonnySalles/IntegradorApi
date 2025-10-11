using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IntegradorApi.Data.Services;

public class DatabaseService {

    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly ILogger _logger;

    public DatabaseService(DbContextOptions<AppDbContext> dbContextOptions, ILogger logger) {
        _dbContextOptions = dbContextOptions;
        _logger = logger;
    }

    public AppDbContext CreateDbContext() {
        return new AppDbContext(_dbContextOptions);
    }

    public async Task<List<Connection>> GetConnectionsAsync() {
        try {
            await using var context = CreateDbContext();
            return await context.Connections.ToListAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao carregar conexões");
            throw;
        }
    }

    public async Task<List<Connection>> GetOriginConnectionsAsync() {
        try {
            await using var context = CreateDbContext();
            return await context.Connections.Where(c => c.TypeDataSource == DataSourceType.ORIGIN).ToListAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao carregar conexões de origem");
            throw;
        }
    }

    public async Task<List<Connection>> GetDestinationConnectionsAsync() {
        try {
            await using var context = CreateDbContext();
            return await context.Connections.Where(c => c.TypeDataSource == DataSourceType.DESTINATION).ToListAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao carregar conexões de destino");
            throw;
        }
    }

    public async Task AddConnectionAsync(Connection connection) {
        try {
            await using var context = CreateDbContext();
            context.Connections.Add(connection);
            await context.SaveChangesAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao adicionar conexão {ConnectionDesc}", connection.Description);
            throw;
        }
    }

    public async Task DeleteConnectionAsync(Connection connection) {
        try {
            await using var context = CreateDbContext();
            context.Connections.Remove(connection);
            await context.SaveChangesAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao deletar conexão {ConnectionDesc}", connection.Description);
            throw;
        }
    }
    public async Task UpdateConnectionAsync(Connection connection) {
        try {
            await using var context = CreateDbContext();
            context.Connections.Update(connection);
            await context.SaveChangesAsync();
        } catch (Exception ex) {
            _logger.Error(ex, "Falha ao atualizar conexão {ConnectionDesc}", connection.Description);
            throw;
        }
    }

    public async Task<DateTime> GetLastSyncDateAsync(int connectionId) {
        await using var context = CreateDbContext();
        var lastSync = await context.Sincronizations
            .Where(s => s.ConnectionId == connectionId)
            .OrderByDescending(s => s.LastSyncronization)
            .FirstOrDefaultAsync();

        return lastSync?.LastSyncronization ?? new DateTime(2000, 1, 1);
    }

    public async Task UpdateLastSyncDateAsync(int connectionId, DateTime syncDate) {
        await using var context = CreateDbContext();

        var syncRecord = new Sincronization {
            ConnectionId = connectionId,
            LastSyncronization = syncDate,
            ResourceIdentifier = "general" // Identificador genérico para a conexão
        };
        context.Sincronizations.Add(syncRecord);
        await context.SaveChangesAsync();
    }
}
