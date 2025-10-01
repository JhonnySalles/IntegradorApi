using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.MangaExtractor;

public class MangaCapa : Entity {
    public string Manga { get; set; } = string.Empty;
    public int Volume { get; set; }
    public Linguagens Lingua { get; set; }
    public string Arquivo { get; set; } = string.Empty;
    public string Extenssao { get; set; } = string.Empty;
    public byte[]? Imagem { get; set; }
    public DateTime? Atualizacao { get; set; }
}