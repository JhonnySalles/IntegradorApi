using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Models.TextoJapones;

namespace IntegradorApi.Sync.Mappings;

public class TextoJaponesMappingProfile : Profile {
    public TextoJaponesMappingProfile() {
        CreateMap<VocabularioDto, VocabularioJapones>().ReverseMap();
        CreateMap<RevisarDto, RevisarJapones>().ReverseMap();
        CreateMap<KanjiInfoDto, KanjiInfo>().ReverseMap();
        CreateMap<KanjaxPtDto, KanjaxPt>().ReverseMap();
        CreateMap<ExclusaoDto, ExclusaoJapones>().ReverseMap();
        CreateMap<EstatisticaDto, EstatisticaJapones>().ReverseMap();
    }
}
