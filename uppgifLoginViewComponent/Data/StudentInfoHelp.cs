using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Areas.Identity.Data;
using uppgifLoginViewComponent.Models;
using uppgifLoginViewComponent.Models.ViewModels;

namespace uppgifLoginViewComponent.Data
{
    public class StudentInfoHelp
    {
        
        private readonly IMapper _mapper;
        public SchoolContext _context;
        public StudentInfoHelp(IMapper mapper, SchoolContext context) 
        {
            
            _context = context;
            _mapper = mapper;
        }

        public StudentInfoViewModel GetStudentInfo(string email) {

            Student student = _context.Students.Where(s => s.Email == email).Include(s => s.Enrollments).ThenInclude(s=> s.Assignments).FirstOrDefault();

            var model = _mapper.Map<StudentInfoViewModel>(student);
            try
            {
                model.CourseAssignment = _context.CourseAssignment.ToList();
            }

            catch (Exception)
            {

                throw;
            }
            return model;
        }
        
    }
}
           
        
