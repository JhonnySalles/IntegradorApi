namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelVocabulario {
    public Guid? Id { get; set; }
    public string Palavra { get; set; } = string.Empty;
    public string Leitura { get; set; } = string.Empty;
    public string Ingles { get; set; } = string.Empty;
    public string Portugues { get; set; } = string.Empty;
    public bool Revisado { get; set; }
    public DateTime? Atualizacao { get; set; }
}