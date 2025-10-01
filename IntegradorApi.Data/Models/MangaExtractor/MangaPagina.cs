namespace IntegradorApi.Data.Models.MangaExtractor;

public class MangaPagina : Entity {
    public string NomePagina { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Hash { get; set; } = string.Empty;
    public List<MangaTexto> Textos { get; set; } = new();
    public HashSet<MangaVocabulario> Vocabularios { get; set; } = new();
    public DateTime? Atualizacao { get; set; }
}