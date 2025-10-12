using IntegradorApi.Data.Models.DeckSubtitle;

namespace IntegradorApi.Data.Repositories.Interfaces;

public interface IDeckSubtitleDao {
    Task UpdateAsync(string dbName, Subtitle obj);
    Task<Guid?> InsertAsync(string dbName, Subtitle obj);
    Task<Subtitle?> SelectAsync(string dbName, Guid id);
    Task<List<Subtitle>> SelectAllAsync(string dbName);
    Task<List<Subtitle>> SelectAllAsync(string dbName, DateTime dateTime);
    Task DeleteAsync(string dbName, Subtitle obj);
    Task CreateTableAsync(string tableName);
    Task<bool> ExistTableAsync(string tableName);
    Task<List<string>> GetTablesAsync();
}