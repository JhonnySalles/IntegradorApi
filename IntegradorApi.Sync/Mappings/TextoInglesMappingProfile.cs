using AutoMapper;
using IntegradorApi.Api.Models.TextoIngles;
using IntegradorApi.Data.Models.TextoIngles;

namespace IntegradorApi.Sync.Mappings;

public class TextoInglesMappingProfile : Profile {
    public TextoInglesMappingProfile() {
        CreateMap<VocabularioDto, VocabularioIngles>().ReverseMap();
        CreateMap<RevisarDto, RevisarIngles>().ReverseMap();
        CreateMap<ExclusaoDto, ExclusaoIngles>().ReverseMap();
        CreateMap<ValidoDto, ValidoIngles>().ReverseMap();
    }
}
