using AutoMapper;
using Restoran.Core.DTOs.Order;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // Entity -> DTO
            CreateMap<Order, OrderListDto>()
                .ForMember(dest => dest.CustomerUsername, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Anonim"));
            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.CustomerUsername, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Anonim"));
            CreateMap<OrderDetail, OrderItemDto>() // OrderDetail entity'sini OrderItemDto'ya map
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Bilinmiyor"));

            // DTO -> Entity
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderItemDto, OrderDetail>(); // OrderItemDto'yu OrderDetail entity'sine map
        }
    }
}
