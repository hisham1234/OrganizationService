using System;
using AutoMapper;
using Organization_Service.Models;
using Organization_Service.Models.DTO;
using Organization_Service.Entities;

namespace Organization_Service.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserEntity, ResponseUserDTO>();
            //CreateMap<UserEntity, UserModel>();
            //CreateMap<OfficeEntity, OfficeModel>();
            //CreateMap<RoleEntity, RoleModel>();
            CreateMap<OfficeEntity, ResponseOfficeDTO>();
            CreateMap<RoleEntity, ResponseRoleDTO>();
        }
    }
}
