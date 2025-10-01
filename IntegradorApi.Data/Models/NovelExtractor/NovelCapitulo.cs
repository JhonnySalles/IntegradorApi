using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelCapitulo : Entity {
    public string Novel { get; set; } = string.Empty;
    public float Volume { get; set; }
    public float Capitulo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Sequencia { get; set; }
    public Linguagens Lingua { get; set; }
    public List<NovelTexto> Textos { get; set; } = new();
    public HashSet<NovelVocabulario> Vocabularios { get; set; } = new();
    public DateTime? Atualizacao { get; set; }
}