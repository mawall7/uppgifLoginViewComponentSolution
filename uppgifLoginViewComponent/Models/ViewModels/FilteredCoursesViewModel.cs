using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models.ViewModels
{
    public class FilteredCoursesViewModel
    {
        //Courses select
        public List<SelectListItem> FilteredCourses { get; set; }
    }
}
