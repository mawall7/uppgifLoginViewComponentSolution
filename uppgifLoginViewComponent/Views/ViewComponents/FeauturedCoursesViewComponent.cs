using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Data;
using uppgifLoginViewComponent.Models;

namespace uppgifLoginViewComponent.Views.ViewComponents
{
    public class FeauturedCoursesViewComponent : ViewComponent
    {
        private SchoolContext _context { get; set; }
        public FeauturedCoursesViewComponent(SchoolContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
            
        }

    }
}
