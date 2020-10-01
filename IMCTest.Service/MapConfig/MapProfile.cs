using AutoMapper;
using IMCTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Taxjar;

namespace IMCTest.Service.MapConfig
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Address, AddressVM>().ReverseMap();

            CreateMap<Order, OrderVM>().ReverseMap();
        }
    }
}
