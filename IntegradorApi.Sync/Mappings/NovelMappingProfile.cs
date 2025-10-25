using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Models.NovelExtractor;

public class NovelMappingProfile : Profile {
    public NovelMappingProfile() {
        // DTO para a Entidade
        CreateMap<NovelVolumeDto, NovelVolume>();
        CreateMap<NovelCapaDto, NovelCapa>();
        CreateMap<NovelCapituloDto, NovelCapitulo>();
        CreateMap<NovelTextoDto, NovelTexto>();
        CreateMap<NovelVocabularioDto, NovelVocabulario>();

        // Entidade para o DTO
        CreateMap<NovelVolume, NovelVolumeDto>();
        CreateMap<NovelCapa, NovelCapaDto>();
        CreateMap<NovelCapitulo, NovelCapituloDto>();
        CreateMap<NovelTexto, NovelTextoDto>();
        CreateMap<NovelVocabulario, NovelVocabularioDto>();
    }
}