namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelTexto {
    public Guid? Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Sequencia { get; set; }
    public DateTime? Atualizacao { get; set; }
}