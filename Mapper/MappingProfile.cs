using AutoMapper;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;

namespace StilSepetiApp.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductCreatedto, Product>()
    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));

            CreateMap<OrderItem, OrderItemdto>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));

           
            CreateMap<Product, Productdto>().ReverseMap();

            CreateMap<ReturnRequest, ReturnRequestdto>().ReverseMap();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
               .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty))
                .ReverseMap();



        }
    }
}