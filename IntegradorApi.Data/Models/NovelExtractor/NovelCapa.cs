using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.NovelExtractor;

public class NovelCapa {
    public Guid? Id { get; set; }
    public string Novel { get; set; } = string.Empty;
    public int Volume { get; set; }
    public Linguagens Lingua { get; set; }
    public string Arquivo { get; set; } = string.Empty;
    public string Extenssao { get; set; } = string.Empty;
    public byte[]? Imagem { get; set; }
    public DateTime? Atualizacao { get; set; }
}