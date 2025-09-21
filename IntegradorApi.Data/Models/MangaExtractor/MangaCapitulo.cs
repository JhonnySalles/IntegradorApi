using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.MangaExtractor;

public class MangaCapitulo {
    public Guid? Id { get; set; }
    public string Manga { get; set; } = string.Empty;
    public int Volume { get; set; }
    public float Capitulo { get; set; }
    public Linguagens Lingua { get; set; }
    public string Scan { get; set; } = string.Empty;
    public List<MangaPagina> Paginas { get; set; } = new();
    public bool IsExtra { get; set; }
    public bool IsRaw { get; set; }
    public HashSet<MangaVocabulario> Vocabularios { get; set; } = new();
    public DateTime? Atualizacao { get; set; }
}