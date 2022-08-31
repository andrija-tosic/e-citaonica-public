using AutoMapper;
using e_citaonica_api.Models;
using e_citaonica_api.TransferModels;

namespace e_citaonica_api.Mapping;

public class DomainToResponseProfile : Profile
{
    public DomainToResponseProfile()
    {
        CreateMap<DiskusijaZadatak, DiskusijaZadatakResponse>();
        CreateMap<DiskusijaOblast, DiskusijaOblastResponse>();
    }
}
