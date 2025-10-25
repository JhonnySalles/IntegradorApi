using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class ComicInfoDto {
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("idMal")]
    public long? IdMal { get; set; }
    [JsonProperty("comic")]
    public string? Comic { get; set; }
    [JsonProperty("title")]
    public string? Title { get; set; }
    [JsonProperty("series")]
    public string? Series { get; set; }
    [JsonProperty("number")]
    public float Number { get; set; }
    [JsonProperty("volume")]
    public int Volume { get; set; }
    [JsonProperty("notes")]
    public string? Notes { get; set; }
    [JsonProperty("year")]
    public int? Year { get; set; }
    [JsonProperty("month")]
    public int? Month { get; set; }
    [JsonProperty("day")]
    public int? Day { get; set; }
    [JsonProperty("writer")]
    public string? Writer { get; set; }
    [JsonProperty("penciller")]
    public string? Penciller { get; set; }
    [JsonProperty("inker")]
    public string? Inker { get; set; }
    [JsonProperty("coverArtist")]
    public string? CoverArtist { get; set; }
    [JsonProperty("colorist")]
    public string? Colorist { get; set; }
    [JsonProperty("letterer")]
    public string? Letterer { get; set; }
    [JsonProperty("publisher")]
    public string? Publisher { get; set; }
    [JsonProperty("tags")]
    public string? Tags { get; set; }
    [JsonProperty("web")]
    public string? Web { get; set; }
    [JsonProperty("editor")]
    public string? Editor { get; set; }
    [JsonProperty("translator")]
    public string? Translator { get; set; }
    [JsonProperty("pageCount")]
    public int? PageCount { get; set; }
    //[JsonProperty("pages")]
    //public List<PagesDto>? Pages { get; set; } // Assumindo que PagesDto existe
    [JsonProperty("count")]
    public int? Count { get; set; }
    [JsonProperty("alternateSeries")]
    public string? AlternateSeries { get; set; }
    [JsonProperty("alternateNumber")]
    public float? AlternateNumber { get; set; }
    [JsonProperty("storyArc")]
    public string? StoryArc { get; set; }
    [JsonProperty("storyArcNumber")]
    public string? StoryArcNumber { get; set; }
    [JsonProperty("seriesGroup")]
    public string? SeriesGroup { get; set; }
    [JsonProperty("alternateCount")]
    public int? AlternateCount { get; set; }
    [JsonProperty("summary")]
    public string? Summary { get; set; }
    [JsonProperty("imprint")]
    public string? Imprint { get; set; }
    [JsonProperty("genre")]
    public string? Genre { get; set; }
    [JsonProperty("languageISO")]
    public string? LanguageISO { get; set; }
    [JsonProperty("format")]
    public string? Format { get; set; }
    //[JsonProperty("ageRating")]
    //public AgeRating? AgeRating { get; set; } // Assumindo que o enum AgeRating existe
    [JsonProperty("communityRating")]
    public float? CommunityRating { get; set; }
    //[JsonProperty("blackAndWhite")]
    //public YesNo? BlackAndWhite { get; set; } // Assumindo que o enum YesNo existe
    //[JsonProperty("manga")]
    //public string? Manga { get; set; } // A API retorna string, converter para enum Manga se necessário
    [JsonProperty("characters")]
    public string? Characters { get; set; }
    [JsonProperty("teams")]
    public string? Teams { get; set; }
    [JsonProperty("locations")]
    public string? Locations { get; set; }
    [JsonProperty("scanInformation")]
    public string? ScanInformation { get; set; }
    [JsonProperty("mainCharacterOrTeam")]
    public string? MainCharacterOrTeam { get; set; }
    [JsonProperty("review")]
    public string? Review { get; set; }
    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }

}
