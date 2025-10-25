using IntegradorApi.Data.Enums.ComicInfo;

namespace IntegradorApi.Data.Models.ProcessaTexto;

public class ComicInfo : Entity {
    public long? IdMal { get; set; }
    public string Comic { get; set; }
    public string Title { get; set; }
    public string Series { get; set; }
    public float Number { get; set; }
    public int Volume { get; set; }
    public string? Notes { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }
    public string? Writer { get; set; }
    public string? Penciller { get; set; }
    public string? Inker { get; set; }
    public string? CoverArtist { get; set; }
    public string? Colorist { get; set; }
    public string? Letterer { get; set; }
    public string? Publisher { get; set; }
    public string? Tags { get; set; }
    public string? Web { get; set; }
    public string? Editor { get; set; }
    public string? Translator { get; set; }
    public int? PageCount { get; set; }
    public List<Pages>? Pages { get; set; }
    public int? Count { get; set; }
    public string? AlternateSeries { get; set; }
    public float? AlternateNumber { get; set; }
    public string? StoryArc { get; set; }
    public string? StoryArcNumber { get; set; }
    public string? SeriesGroup { get; set; }
    public int? AlternateCount { get; set; }
    public string? Summary { get; set; }
    public string? Imprint { get; set; }
    public string? Genre { get; set; }
    public string LanguageISO { get; set; }
    public string? Format { get; set; }
    public AgeRating? AgeRating { get; set; }
    public float? CommunityRating { get; set; }
    public YesNo? BlackAndWhite { get; set; }
    public Manga Manga { get; set; }
    public string? Characters { get; set; }
    public string? Teams { get; set; }
    public string? Locations { get; set; }
    public string? ScanInformation { get; set; }
    public string? MainCharacterOrTeam { get; set; }
    public string? Review { get; set; }

    public ComicInfo() {
        Comic = string.Empty;
        Title = string.Empty;
        Series = string.Empty;
        LanguageISO = string.Empty;
        Manga = Manga.Yes;
    }

    public ComicInfo(Guid? id, long? idMal, string comic, string title, string series, string? publisher, string? alternateSeries,
        string? storyArc, string? seriesGroup, string? imprint, string? genre, string languageISO, AgeRating? ageRating) {
        this.Id = id;
        this.IdMal = idMal;
        this.Comic = comic;
        this.Title = title;
        this.Series = series;
        this.Publisher = publisher;
        this.AlternateSeries = alternateSeries;
        this.StoryArc = storyArc;
        this.SeriesGroup = seriesGroup;
        this.Imprint = imprint;
        this.Genre = genre;
        this.LanguageISO = languageISO;
        this.AgeRating = ageRating;
        this.Manga = Manga.Yes;
    }

    public static ComicInfo Create(Guid? id) {
        return new ComicInfo { Id = id };
    }

    public void Merge(ComicInfo comic) {
        this.Id = comic.Id;
        this.IdMal = comic.IdMal;
        this.Comic = comic.Comic;
        this.Title = comic.Title;
        this.Series = comic.Series;
        this.Publisher = comic.Publisher;
        this.AlternateSeries = comic.AlternateSeries;
        this.StoryArc = comic.StoryArc;
        this.SeriesGroup = comic.SeriesGroup;
        this.Imprint = comic.Imprint;
        this.Genre = comic.Genre;
        this.LanguageISO = comic.LanguageISO;
        this.AgeRating = comic.AgeRating;
    }

    public void Patch(ComicInfo source) {
        if (source.IdMal != null)
            this.IdMal = source.IdMal;

        if (!string.IsNullOrEmpty(source.Comic))
            this.Comic = source.Comic;

        if (!string.IsNullOrEmpty(source.Title))
            this.Title = source.Title;

        if (!string.IsNullOrEmpty(source.Series))
            this.Series = source.Series;

        if (source.Publisher != null)
            this.Publisher = source.Publisher;

        if (source.AlternateSeries != null)
            this.AlternateSeries = source.AlternateSeries;

        if (source.StoryArc != null)
            this.StoryArc = source.StoryArc;

        if (source.SeriesGroup != null)
            this.SeriesGroup = source.SeriesGroup;

        if (source.Imprint != null)
            this.Imprint = source.Imprint;

        if (source.Genre != null)
            this.Genre = source.Genre;

        if (!string.IsNullOrEmpty(source.LanguageISO))
            this.LanguageISO = source.LanguageISO;

        if (source.AgeRating != null)
            this.AgeRating = source.AgeRating;
    }
}