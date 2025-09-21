namespace IntegradorApi.Data.Models.MangaExtractor;

public class MangaTexto {
    public Guid? Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Sequencia { get; set; }
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public DateTime? Atualizacao { get; set; }
}