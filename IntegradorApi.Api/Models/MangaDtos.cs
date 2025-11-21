using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class MangaVolumeDto {
    public Guid Id { get; set; }
    public string Manga { get; set; }
    public int Volume { get; set; }
    public string? Lingua { get; set; }
    public List<MangaCapituloDto> Capitulos { get; set; } = new();
    public List<MangaVocabularioDto> Vocabularios { get; set; } = new();
    public string? Arquivo { get; set; }
    public bool Processado { get; set; }
    public MangaCapaDto? Capa { get; set; }
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class MangaCapituloDto {
    public Guid Id { get; set; }
    public string? Manga { get; set; }
    public int Volume { get; set; }
    public float Capitulo { get; set; }
    public string? Lingua { get; set; }
    public string? Scan { get; set; }
    public List<MangaPaginaDto> Paginas { get; set; } = new();
    public bool IsExtra { get; set; }
    public bool IsRaw { get; set; }
    public List<MangaVocabularioDto> Vocabularios { get; set; } = new();
    public DateTime Atualizacao { get; set; }
}

public class MangaPaginaDto {
    public Guid Id { get; set; }
    public string? NomePagina { get; set; }
    public int Numero { get; set; }
    public string? Hash { get; set; }
    public List<MangaTextoDto> Textos { get; set; } = new();
    public List<MangaVocabularioDto> Vocabularios { get; set; } = new();
    public DateTime Atualizacao { get; set; }
}

public class MangaTextoDto {
    public Guid Id { get; set; }
    public string? Texto { get; set; }
    public int Sequencia { get; set; }
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public DateTime Atualizacao { get; set; }
}

public class MangaCapaDto {
    public Guid Id { get; set; }
    public string? Manga { get; set; }
    public int Volume { get; set; }
    public string? Lingua { get; set; }
    public string? Arquivo { get; set; }
    public string? Extenssao { get; set; }
    public string? Imagem { get; set; }
    public DateTime Atualizacao { get; set; }
}

public class MangaVocabularioDto {
    // A estrutura do vocabulário precisa ser definida baseada na API,
    // por enquanto, criamos uma classe vazia ou com campos conhecidos.
    public Guid Id { get; set; }
    public string? Palavra { get; set; }
}