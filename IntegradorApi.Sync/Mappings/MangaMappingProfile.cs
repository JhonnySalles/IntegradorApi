using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Models.MangaExtractor;

public class MangaMappingProfile : Profile {
    public MangaMappingProfile() {
        // DTO para a Entidade
        CreateMap<MangaVolumeDto, MangaVolume>();
        CreateMap<MangaCapaDto, MangaCapa>();
        CreateMap<MangaCapituloDto, MangaCapitulo>();
        CreateMap<MangaPaginaDto, MangaPagina>();
        CreateMap<MangaTextoDto, MangaTexto>();
        CreateMap<MangaVocabularioDto, MangaVocabulario>();

        // Entidade para o DTO
        CreateMap<MangaVolume, MangaVolumeDto>();
        CreateMap<MangaCapa, MangaCapaDto>();
        CreateMap<MangaCapitulo, MangaCapituloDto>();
        CreateMap<MangaPagina, MangaPaginaDto>();
        CreateMap<MangaTexto, MangaTextoDto>();
        CreateMap<MangaVocabulario, MangaVocabularioDto>();
    }
}