using AutoMapper;
using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Application.Mappings
{
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                //this line maps matching names
                CreateMap<CreateProductDto, Product>();

                CreateMap<Product, ProductDto>();
            }
        }
}
