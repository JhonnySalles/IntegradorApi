using IntegradorApi.Data;
using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Services;

public class DatabaseService {

    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public DatabaseService(DbContextOptions<AppDbContext> dbContextOptions) {
        _dbContextOptions = dbContextOptions;
    }

    public AppDbContext CreateDbContext() {
        return new AppDbContext(_dbContextOptions);
    }

    public async Task<List<Connection>> GetConnectionsAsync() {
        await using var context = CreateDbContext();
        return await context.Connections.ToListAsync();
    }

    public async Task AddConnectionAsync(Connection connection) {
        await using var context = CreateDbContext();
        context.Connections.Add(connection);
        await context.SaveChangesAsync();
    }

    public async Task DeleteConnectionAsync(Connection connection) {
        await using var context = CreateDbContext();
        context.Connections.Remove(connection);
        await context.SaveChangesAsync();
    }
    public async Task UpdateConnectionAsync(Connection connectionToUpdate) {
        await using var context = CreateDbContext();
        context.Connections.Update(connectionToUpdate);
        await context.SaveChangesAsync();
    }
}
