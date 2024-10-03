using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models
{
    public class AssignmentViewModel
    {
        //public IFormFile File { get; set; }
        public byte[] File { get; set; }
        public int StudentID { get; set; }
        public string StudentLastName { get; set; }
        public int CourseIDAssignment { get; set; }


    }
}
