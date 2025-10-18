using IntegradorApi.Data.Models.NovelExtractor;

namespace IntegradorApi.Data.Repositories.Interfaces;

public interface INovelExtractorDao {
    // UPDATE
    Task<NovelCapa> UpdateCapaAsync(string dbName, NovelCapa obj);
    Task<NovelCapitulo> UpdateCapituloAsync(string dbName, NovelCapitulo obj);
    Task<NovelTexto> UpdateTextoAsync(string dbName, NovelTexto obj);
    Task<NovelVolume> UpdateVolumeAsync(string dbName, NovelVolume obj);

    // INSERT
    Task<Guid?> InsertCapaAsync(string dbName, Guid idVolume, NovelCapa obj);
    Task<Guid?> InsertCapituloAsync(string dbName, Guid idVolume, NovelCapitulo obj);
    Task<Guid?> InsertTextoAsync(string dbName, Guid idCapitulo, NovelTexto obj);
    Task<Guid?> InsertVolumeAsync(string dbName, NovelVolume obj);

    // SELECT
    Task<NovelCapa?> SelectCapaAsync(string dbName, Guid idVolume);
    Task<NovelCapitulo?> SelectCapituloAsync(string dbName, Guid id);
    Task<NovelTexto?> SelectTextoAsync(string dbName, Guid id);
    Task<NovelVolume?> SelectVolumeAsync(string dbName, Guid id);
    Task<List<NovelCapitulo>> SelectAllCapitulosAsync(string dbName, Guid idVolume);
    Task<List<NovelTexto>> SelectAllTextosAsync(string dbName, Guid idCapitulo);
    Task<List<NovelVolume>> SelectAllVolumesAsync(string dbName);
    Task<List<NovelVolume>> SelectAllVolumesAsync(string dbName, DateTime since);

    // DELETE
    Task DeleteCapituloAsync(string dbName, NovelCapitulo obj);
    Task DeleteVolumeAsync(string dbName, NovelVolume obj);

    // VOCABULARIO
    Task<HashSet<NovelVocabulario>> SelectVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null);
    Task InsertVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, ICollection<NovelVocabulario>? vocabulario = null);
    Task DeleteVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, bool transaction = true);

    // Tabelas
    Task CreateTableAsync(string tableName);
    Task<bool> ExistTableAsync(string tableName);
    Task<List<string>> GetTablesAsync();
    Task<List<string>> GetTablesAsync(DateTime since);

    // Existe
    Task<bool> ExistVolumeAsync(string tableName, Guid id);
}