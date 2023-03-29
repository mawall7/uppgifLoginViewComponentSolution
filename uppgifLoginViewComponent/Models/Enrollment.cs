namespace uppgifLoginViewComponent.Models
{
    public class Enrollment
    {
         public enum Grade
        {
            A,B,C,D,E,F
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public Grade? StudentGrade { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }





    }
}