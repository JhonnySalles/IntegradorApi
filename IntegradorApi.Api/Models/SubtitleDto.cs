using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class SubtitleDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("episodio")]
    public int Episodio { get; set; }
    //[JsonProperty("linguagem")]
    //public string? Linguagem { get; set; } // Ou usar o enum Linguagens se apropriado
    [JsonProperty("tempoInicial")]
    public string? TempoInicial { get; set; }
    [JsonProperty("tempoFinal")]
    public string? TempoFinal { get; set; }
    [JsonProperty("texto")]
    public string? Texto { get; set; }
    [JsonProperty("traducao")]
    public string? Traducao { get; set; }
    [JsonProperty("vocabulario")]
    public string? Vocabulario { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }
}
