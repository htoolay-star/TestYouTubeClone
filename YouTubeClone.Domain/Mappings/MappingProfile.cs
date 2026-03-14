using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Shared.DTOs;
using YouTubeClone.Shared.DTOs.Cache;

namespace YouTubeClone.Domain.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // RegisterRequest (DTO) ကနေ User (Entity) ကို Map လုပ်မယ်
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password ကို manual hash လုပ်မှာမို့ ignore ထားမယ်
                .ForMember(dest => dest.PublicId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // UpdateUserRequest အတွက်လည်း တစ်ခါတည်း ဆောက်ထားလို့ရပါတယ် (null ကို ignore လုပ်တဲ့ logic နဲ့)
            CreateMap<UpdateUserRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Entities.User, UserCacheItem>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName)));
        }
    }
}
