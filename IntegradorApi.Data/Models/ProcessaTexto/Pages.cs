using IntegradorApi.Data.Enums.ComicInfo;

namespace IntegradorApi.Data.Models.ProcessaTexto;

public class Pages {
    public string? Bookmark { get; set; }

    public int? Image { get; set; }

    public int? ImageHeight { get; set; }

    public int? ImageWidth { get; set; }

    public long? ImageSize { get; set; }

    public ComicPageType? Type { get; set; }

    public bool? DoublePage { get; set; }

    public string? Key { get; set; }
}