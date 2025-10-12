using IntegradorApi.Data.Models.ProcessaTexto;

namespace IntegradorApi.Data.Repositories.Interfaces;

public interface IComicInfoDao {
    Task SaveAsync(ComicInfo obj);
    Task<ComicInfo?> FindAsync(string comic, string linguagem);
    Task<ComicInfo?> FindAsync(Guid id, string comic, string linguagem);
    Task<List<ComicInfo>> FindForUpdateAsync(DateTime lastUpdate);
}