using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models
{
    public class Assignment
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int CourseAssignmentID { get; set; }
        
        public string Name { get; set; }
        public byte[] AssignmentFile { get; set; }
        public int EnrollmentID { get; set; }
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SubmissionDate { get; set; } 
        
        //public CourseAssignment CourseAssignment { get; set; }
        

        


        




    }

}
