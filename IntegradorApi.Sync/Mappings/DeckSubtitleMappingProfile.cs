using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Models.DeckSubtitle;

public class DeckSubtitleMappingProfile : Profile {
    public DeckSubtitleMappingProfile() {
        // DTO para a Entidade
        CreateMap<SubtitleDto, Subtitle>();

        // Entidade para o DTO
        CreateMap<Subtitle, SubtitleDto>();
    }
}