using IntegradorApi.Data;
using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Services;

public class DatabaseService {

    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public DatabaseService(DbContextOptions<AppDbContext> dbContextOptions) {
        _dbContextOptions = dbContextOptions;
    }

    private AppDbContext CreateDbContext() {
        return new AppDbContext(_dbContextOptions);
    }

    public async Task<List<Connection>> GetConnectionsAsync() {
        await using var context = new AppDbContext();
        return await context.DbSetConnections.ToListAsync();
    }

    public async Task AddConnectionAsync(Connection connection) {
        await using var context = new AppDbContext();
        context.DbSetConnections.Add(connection);
        await context.SaveChangesAsync();
    }

    public async Task DeleteConnectionAsync(Connection connection) {
        await using var context = new AppDbContext();
        context.DbSetConnections.Remove(connection);
        await context.SaveChangesAsync();
    }
    public async Task UpdateConnectionAsync(Connection connectionToUpdate) {
        await using var context = new AppDbContext();
        context.DbSetConnections.Update(connectionToUpdate);
        await context.SaveChangesAsync();
    }
}
