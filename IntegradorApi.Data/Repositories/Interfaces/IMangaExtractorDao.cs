using IntegradorApi.Data.Models.MangaExtractor;

namespace IntegradorApi.Data.Repositories.Interfaces;

public interface IMangaExtractorDao {
    // UPDATE
    Task<MangaCapa> UpdateCapaAsync(string dbName, MangaCapa obj);
    Task<MangaCapitulo> UpdateCapituloAsync(string dbName, MangaCapitulo obj);
    Task<MangaPagina> UpdatePaginaAsync(string dbName, MangaPagina obj);
    Task<MangaTexto> UpdateTextoAsync(string dbName, MangaTexto obj);
    Task<MangaVolume> UpdateVolumeAsync(string dbName, MangaVolume obj);

    // INSERT
    Task<Guid?> InsertCapaAsync(string dbName, Guid idVolume, MangaCapa obj);
    Task<Guid?> InsertCapituloAsync(string dbName, Guid idVolume, MangaCapitulo obj);
    Task<Guid?> InsertPaginaAsync(string dbName, Guid idCapitulo, MangaPagina obj);
    Task<Guid?> InsertTextoAsync(string dbName, Guid idPagina, MangaTexto obj);
    Task<Guid?> InsertVolumeAsync(string dbName, MangaVolume obj);

    // SELECT
    Task<MangaCapa?> SelectCapaAsync(string dbName, Guid idVolume);
    Task<MangaCapitulo?> SelectCapituloAsync(string dbName, Guid id);
    Task<MangaPagina?> SelectPaginaAsync(string dbName, Guid id);
    Task<MangaTexto?> SelectTextoAsync(string dbName, Guid id);
    Task<MangaVolume?> SelectVolumeAsync(string dbName, Guid id);
    Task<List<MangaCapitulo>> SelectAllCapitulosAsync(string dbName, Guid idVolume);
    Task<List<MangaPagina>> SelectAllPaginasAsync(string dbName, Guid idCapitulo);
    Task<List<MangaTexto>> SelectAllTextosAsync(string dbName, Guid idPagina);
    Task<List<MangaVolume>> SelectAllVolumesAsync(string dbName);
    Task<List<MangaVolume>> SelectAllVolumesAsync(string dbName, DateTime since);

    // DELETE
    Task DeleteCapituloAsync(string dbName, MangaCapitulo obj);
    Task DeleteVolumeAsync(string dbName, MangaVolume obj);

    // VOCABULARIO
    Task<HashSet<MangaVocabulario>> SelectVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, Guid? idPagina = null);
    Task InsertVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, Guid? idPagina = null, ICollection<MangaVocabulario>? vocabulario = null);
    Task DeleteVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, Guid? idPagina = null);

    // Tabelas
    Task CreateTableAsync(string tableName);
    Task<bool> ExistTableAsync(string tableName);
    Task<List<string>> GetTablesAsync();
    Task<List<string>> GetTablesAsync(DateTime since);

    // Existe
    Task<bool> ExistVolumeAsync(string tableName, Guid id);
}