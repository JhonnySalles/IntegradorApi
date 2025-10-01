namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelTexto : Entity {
    public string Texto { get; set; } = string.Empty;
    public int Sequencia { get; set; }
    public DateTime? Atualizacao { get; set; }
}