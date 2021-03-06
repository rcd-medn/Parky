﻿



using AutoMapper;
using ParkyAPI.Models.DTOs;

namespace ParkyAPI.Models.ParkyMapper
{
    public class ParkyMappings : Profile
    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDTO>().ReverseMap();
            CreateMap<Trail, TrailDTO>().ReverseMap();
            CreateMap<Trail, TrailCreateDTO>().ReverseMap();
            CreateMap<Trail, TrailUpdatetDTO>().ReverseMap();
        }
    }
}
