﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    public class GetProductProfile : Profile
    {
        public GetProductProfile()
        {
            CreateMap<Product, GetProductResult>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

            CreateMap<Domain.Entities.Rating, Rating>();
        }
    }
}