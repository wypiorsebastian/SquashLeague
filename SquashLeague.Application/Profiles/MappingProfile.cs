using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using SquashLeague.Application.CQRS.Seasons.DTOs;
using SquashLeague.Domain.Entities;

namespace SquashLeague.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Season, SeasonDTO>().ReverseMap();
        }
    }
}
