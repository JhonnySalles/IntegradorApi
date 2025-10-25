using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Models.ProcessaTexto;

public class ComicInfoMappingProfile : Profile {
    public ComicInfoMappingProfile() {
        // DTO para a Entidade
        CreateMap<ComicInfoDto, ComicInfo>();

        // Entidade para o DTO
        CreateMap<ComicInfo, ComicInfoDto>();
    }
}