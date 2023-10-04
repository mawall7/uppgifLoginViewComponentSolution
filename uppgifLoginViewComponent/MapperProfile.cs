using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models;
using uppgifLoginViewComponent.Models.ViewModels;

namespace uppgifLoginViewComponent
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Student, StudentInfoViewModel>();
        }

    }
}
