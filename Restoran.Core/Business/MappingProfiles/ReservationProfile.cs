using AutoMapper;
using Restoran.Core.DTOs.Reservation;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business.MappingProfiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Entity -> DTO  
            CreateMap<Reservation, ReservationListDto>()
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table != null ? int.Parse(src.Table.TableNumber) : 0));

            CreateMap<Reservation, ReservationDetailDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Bilinmiyor"))
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table != null ? int.Parse(src.Table.TableNumber) : 0));

            // DTO -> Entity  
            CreateMap<ReservationCreateDto, Reservation>();
            CreateMap<ReservationUpdateDto, Reservation>();
        }
    }
}
