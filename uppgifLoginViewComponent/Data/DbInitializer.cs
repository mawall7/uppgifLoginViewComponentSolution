using uppgifLoginViewComponent.Models;
using System;
using System.Linq;
using uppgifLoginViewComponent.Data;
using static uppgifLoginViewComponent.Models.Enrollment;


namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Enrollments.Any())
            {
                return;   // DB has been seeded
            }
            else
            {
                var students = new Student[]
                {
                    new Student{  FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01"), Email="a@aaaaaa.com", PostalCode="11122 vägen"},
                    new Student{  FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01"), Email="a@aaaaaa.as", PostalCode="11122 vägen"},

                };
                foreach (Student s in students)
                {
                    context.Students.Add(s);
                }
                context.SaveChanges();

                var courses = new Course[]
                {
            new Course{Title="Chemistry",Credits=3},
            new Course{Title="Microeconomics",Credits=3},

                };
                foreach (Course c in courses)
                {
                    context.Courses.Add(c);
                }
                context.SaveChanges();

                var enrollments = new Enrollment[]
                {
            new Enrollment{CourseID=1, StudentID=1,StudentGrade=Grade.A},
            new Enrollment{StudentID=2, CourseID=1, StudentGrade=Grade.C},
            new Enrollment{StudentID=1, CourseID=2, StudentGrade=Grade.B},

                };
                foreach (Enrollment e in enrollments)
                {
                    context.Enrollments.Add(e);
                }
                context.SaveChanges();
            }
        }
    }
}
