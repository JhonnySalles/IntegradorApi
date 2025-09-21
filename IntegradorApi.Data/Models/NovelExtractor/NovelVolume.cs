using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelVolume {
    public Guid? Id { get; set; }
    public string Novel { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string TituloAlternativo { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Arquivo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public float Volume { get; set; }
    public Linguagens Lingua { get; set; }
    public bool Favorito { get; set; }
    public bool Processado { get; set; }
    public NovelCapa? Capa { get; set; }
    public List<NovelCapitulo> Capitulos { get; set; } = new();
    public HashSet<NovelVocabulario> Vocabularios { get; set; } = new();
    public DateTime? Atualizacao { get; set; }
}