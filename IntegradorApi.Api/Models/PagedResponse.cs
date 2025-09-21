using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

/// <summary>
/// Representa uma resposta de API paginada e genérica no padrão HATEOAS.
/// 'T' será o tipo do objeto de dados, como MangaVolumeDto ou NovelVolumeDto.
/// </summary>
public class PagedApiResponse<T> {
    [JsonProperty("_embedded")]
    public EmbeddedContent<T>? Embedded { get; set; }

    [JsonProperty("_links")]
    public HateoasLinks? Links { get; set; }

    [JsonProperty("page")]
    public PageInfo? Page { get; set; }

    /// <summary>
    /// Uma propriedade de atalho para acessar a lista de conteúdo diretamente,
    /// independentemente do nome da chave no JSON (ex: "mangaVolumeDtoList").
    /// </summary>
    [JsonIgnore] // Ignora esta propriedade na serialização/deserialização direta
    public List<T> Content => Embedded?.ContentList.FirstOrDefault() ?? new List<T>();
}

/// <summary>
/// Representa o conteúdo "_embedded" onde a chave (ex: "mangaVolumeDtoList") é dinâmica.
/// </summary>
public class EmbeddedContent<T> {
    /// <summary>
    /// Usa [JsonExtensionData] para capturar qualquer chave que não seja mapeada diretamente.
    /// O resultado será um dicionário onde a chave é o nome da lista (ex: "mangaVolumeDtoList")
    /// e o valor é a própria lista de objetos.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }

    [JsonIgnore]
    public IEnumerable<List<T>> ContentList {
        get {
            if (ExtensionData == null)
                return Enumerable.Empty<List<T>>();

            return ExtensionData.Values
                .OfType<Newtonsoft.Json.Linq.JArray>()
                .Select(jArray => jArray.ToObject<List<T>>()!);
        }
    }
}

public class HateoasLinks {
    public LinkHref? First { get; set; }
    public LinkHref? Self { get; set; }
    public LinkHref? Next { get; set; }
    public LinkHref? Last { get; set; }
}

public class LinkHref {
    public string? Href { get; set; }
}

public class PageInfo {
    public int Size { get; set; }
    public int TotalElements { get; set; }
    public int TotalPages { get; set; }
    public int Number { get; set; }
}