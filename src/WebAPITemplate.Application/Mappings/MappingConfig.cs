using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPITemplate.Application.DTOs.Responses;
using WebAPITemplate.Application.Models;

namespace WebAPITemplate.Application.Mappings
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<UserModel, UserResponseDto>
                .NewConfig()
                .Map(dest => dest.FullName, src => src.Name)
                .Map(dest => dest.EmailAddress, src => src.Email);
        }
    }
}
