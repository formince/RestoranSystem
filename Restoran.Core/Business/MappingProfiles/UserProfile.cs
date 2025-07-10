using AutoMapper;
using Restoran.Core.DTOs.User;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity -> DTO
            CreateMap<User, UserListDto>();
            CreateMap<User, UserDetailDto>();

            // DTO -> Entity
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password hash'leme serviste yapılacak
            CreateMap<UserUpdateDto, User>();
            // UserLoginDto'yu User entity'sine maplemeye gerek yok, o sadece kimlik doğrulama içindir
        }
    }
}
