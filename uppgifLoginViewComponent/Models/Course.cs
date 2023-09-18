using System.Collections.Generic;

namespace uppgifLoginViewComponent.Models
{
    public class Course
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }

        public ICollection<CourseAssignment> CourseAssignments {get; set;}

    }
}
