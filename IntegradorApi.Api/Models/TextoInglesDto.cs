using Newtonsoft.Json;
using IntegradorApi.Api.Models;

namespace IntegradorApi.Api.Models.TextoIngles;

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

public class RevisarDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("vocabulario")]
    public string? Vocabulario { get; set; }
    [JsonProperty("leitura")]
    public string? Leitura { get; set; }
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

public class ValidoDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("palavra")]
    public string? Palavra { get; set; }
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
    [JsonProperty("leitura")]
    public string? Leitura { get; set; }
    [JsonProperty("portugues")]
    public string? Portugues { get; set; }
    [JsonProperty("atualizacao")]
    public DateTime Atualizacao { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}
