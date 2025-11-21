using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class NovelVolumeDto {
    public Guid Id { get; set; }
    public string? Novel { get; set; }
    public string? Titulo { get; set; }
    public string? TituloAlternativo { get; set; }
    public string? Serie { get; set; }
    public string? Descricao { get; set; }
    public string? Arquivo { get; set; }
    public string? Editora { get; set; }
    public string? Autor { get; set; }
    public float Volume { get; set; }
    public string? Lingua { get; set; }
    public bool Favorito { get; set; }
    public bool Processado { get; set; }
    public NovelCapaDto? Capa { get; set; }
    public List<NovelCapituloDto> Capitulos { get; set; } = new();
    public List<NovelVocabularioDto> Vocabularios { get; set; } = new();
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class NovelCapaDto {
    public Guid Id { get; set; }
    public string? Novel { get; set; }
    public int Volume { get; set; }
    public string? Lingua { get; set; }
    public string? Arquivo { get; set; }
    public string? Extenssao { get; set; }
    public string? Imagem { get; set; }
    public DateTime Atualizacao { get; set; }
}

public class NovelCapituloDto {
    public Guid Id { get; set; }
    public string? Novel { get; set; }
    public float Volume { get; set; }
    public float Capitulo { get; set; }
    public string? Descricao { get; set; }
    public int Sequencia { get; set; }
    public string? Lingua { get; set; }
    public List<NovelTextoDto> Textos { get; set; } = new();
    public List<NovelVocabularioDto> Vocabularios { get; set; } = new();
    public DateTime Atualizacao { get; set; }
}

public class NovelTextoDto {
    public Guid Id { get; set; }
    public string? Texto { get; set; }
    public int Sequencia { get; set; }
    public DateTime Atualizacao { get; set; }
}

public class NovelVocabularioDto {
    public Guid Id { get; set; }
    public string? Palavra { get; set; }
}