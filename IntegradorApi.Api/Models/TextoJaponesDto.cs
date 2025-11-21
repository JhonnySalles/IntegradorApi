using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class EstatisticaDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("sequencial")]
    public int Sequencial { get; set; }
    [JsonProperty("kanji")]
    public string? Kanji { get; set; }
    [JsonProperty("leitura")]
    public string? Leitura { get; set; }
    [JsonProperty("tipo")]
    public string? Tipo { get; set; }
    [JsonProperty("quantidade")]
    public double Quantidade { get; set; }
    [JsonProperty("percentual")]
    public double Percentual { get; set; }
    [JsonProperty("media")]
    public double Media { get; set; }
    [JsonProperty("percentualMedio")]
    public double PercentualMedio { get; set; }
    [JsonProperty("corSequencial")]
    public int CorSequencial { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class ExclusaoDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("exclusao")]
    public string? Exclusao { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class KanjiInfoDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("sequencia")]
    public int Sequencia { get; set; }
    [JsonProperty("word")]
    public string? Word { get; set; }
    [JsonProperty("readInfo")]
    public string? ReadInfo { get; set; }
    [JsonProperty("frequency")]
    public int Frequency { get; set; }
    [JsonProperty("tabela")]
    public string? Tabela { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class RevisarDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("vocabulario")]
    public string? Vocabulario { get; set; }
    [JsonProperty("formaBasica")]
    public string? FormaBasica { get; set; }
    [JsonProperty("leitura")]
    public string? Leitura { get; set; }
    [JsonProperty("ingles")]
    public string? Ingles { get; set; }
    [JsonProperty("portugues")]
    public string? Portugues { get; set; }
    [JsonProperty("revisado")]
    public bool Revisado { get; set; }
    [JsonProperty("aparece")]
    public int Aparece { get; set; }
    [JsonProperty("isAnime")]
    public bool IsAnime { get; set; }
    [JsonProperty("isManga")]
    public bool IsManga { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}

public class VocabularioDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("vocabulario")]
    public string? Vocabulario { get; set; }
    [JsonProperty("formaBasica")]
    public string? FormaBasica { get; set; }
    [JsonProperty("leitura")]
    public string? Leitura { get; set; }
    [JsonProperty("leituraNovel")]
    public string? LeituraNovel { get; set; }
    [JsonProperty("ingles")]
    public string? Ingles { get; set; }
    [JsonProperty("portugues")]
    public string? Portugues { get; set; }
    [JsonProperty("jlpt")]
    public int Jlpt { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}
