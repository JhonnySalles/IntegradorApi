using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.MangaExtractor;

public class MangaVolume {
    public Guid? Id { get; set; }
    public string Manga { get; set; } = string.Empty;
    public int Volume { get; set; }
    public Linguagens Lingua { get; set; }
    public List<MangaCapitulo> Capitulos { get; set; } = new();
    public HashSet<MangaVocabulario> Vocabularios { get; set; } = new();
    public string Arquivo { get; set; } = string.Empty;
    public bool Processado { get; set; }
    public MangaCapa? Capa { get; set; }
    public DateTime? Atualizacao { get; set; }
}