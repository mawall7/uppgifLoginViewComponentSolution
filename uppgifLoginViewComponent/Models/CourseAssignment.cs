using System;
using System.ComponentModel.DataAnnotations;

namespace uppgifLoginViewComponent.Models
{
    public class CourseAssignment
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public string AssignmentName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastSubmissionDate { get; set; }
       

    }
}