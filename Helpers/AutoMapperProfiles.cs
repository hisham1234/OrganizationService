using System;
using AutoMapper;
using Organization_Service.Entities;

namespace Organization_Service.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserEntity, UserResponseDTO>();
        }
    }
}
