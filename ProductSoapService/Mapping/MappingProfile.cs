using AutoMapper;
using ProductSoapService.Dtos;
using ProductSoapService.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}