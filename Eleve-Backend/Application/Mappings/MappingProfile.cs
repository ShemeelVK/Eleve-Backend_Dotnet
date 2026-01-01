using AutoMapper;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Domain.ValueObjects;

namespace Eleve_Backend.Application.Mappings
{
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                //this line maps matching names
                CreateMap<CreateProductDto, Product>();

                CreateMap<Product, ProductDto>();

                CreateMap<Address, AddressDto>();
                
                CreateMap<OrderItem,OrderItemResponseDto>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl));

            CreateMap<Order, OrderResponseDto>()
                //converting enum status to string
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            }
        }
}
