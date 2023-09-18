using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using uppgifLoginViewComponent.Data;
using uppgifLoginViewComponent.Models;
using uppgifLoginViewComponent.Models.SchoolViewModels;

namespace uppgifLoginViewComponent.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<StudentsController> _logger;
        public string Enrollname { get; set; }
        public StudentsController(ILogger<StudentsController> logger, SchoolContext context)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)//string är nullable per default är null första träffen
        {




            //obs ViewData adderas hela tiden med en ny key så där CurrentFilter sätts till null har den redan två keys. 
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;

            }
            else
            {
                searchString = currentFilter;
            }

            //var students = from s in _context.Students
            //select s;
            var students = _context.Students.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            int pagesize = 4;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pagesize));//pagesize = antal items som ska vara med på varje sida låter en metod returnera studenter sida för sida
            //return View(await students.AsNoTracking().
            //    ToListAsync());
        }

        // GET: Students/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    return View();
        //}

        public async Task<IActionResult> StudentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student s = await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.ID == id);
            return View(s);
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var student = await _context.Students
        //        .Include(s => s.Enrollments)
        //            .ThenInclude(e => e.Course)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(m => m.ID == id);

        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(student);
        //}

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate, Email, PostalCode")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes." + "If the problem persists contact admnistrator");

            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> EditPost(int? id/*Student student*/)
        {

            //_context.Students.Update(student);//undvik det här för att hindra overposting posta fält som inte är inkluderade i en fält via fiddler tool som inte är tänkta att postas men finns i en student entity
            
            if (ModelState.IsValid)
            {
                var StudenttoUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
                //trypudatemodel fungerar nu, inte när den var i en try catch som i contoso university tutorial försökte hitta felet
                var updateresult = TryUpdateModelAsync<Student>(
                       StudenttoUpdate, "",
                       s => s.LastName, s => s.FirstMidName, s => s.EnrollmentDate, s => s.PostalCode, s => s.Email);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Unable to do changes. If the problem persists contact administrator");
            }
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteCourse(int id)
        {

            try
            {

                var enr = await _context.Enrollments.Where(e => e.ID == id).FirstOrDefaultAsync();
                Student s = await _context.Students.FindAsync(enr.StudentID);
                int sid = s.ID;

                _context.Enrollments.Remove(enr);
                _context.SaveChanges();
                return View(nameof(StudentDetails), s);

            }


            catch (Exception e)
            {
                _logger.Log(LogLevel.Information, e.Message);
            }
            ViewBag.DelCourseMessage = "Course Deleted";

            return View();
            
        }


    

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? SaveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            if (SaveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMesssage"] = "Delete failed. If the error persists contat administrator";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]  //sent with form taghelper 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
              } 
            }
        private bool StudentExists(int id )
        {
            
           
            return _context.Students.Any(e => e.ID == id);
        }

        public async Task<IActionResult> About()
        {
            IQueryable<EnrollmentDateGroup> data = _context.Students.GroupBy(a => a.EnrollmentDate).Select(a =>
               new EnrollmentDateGroup() { EnrollmentDate = a.Key, StudentCount = a.Count()});
            return View(await data.AsNoTracking().ToListAsync());
        }
        public IActionResult Grades()
        {
            List<StudentGradeViewModel> gradelist = GroupEnrollments();
            return View(gradelist);
        }


        public List<StudentGradeViewModel> GroupEnrollments() {
            Student Student = new Student() { FirstMidName = "Name", LastName = "LastName", ID = 77 };
            List<Enrollment> stud = _context.Enrollments.ToList();
            IEnumerable<IGrouping<string, Enrollment>> enrolls = stud.GroupBy(a => a.Name);
            List<int> grademean = new List<int>();
            int enrollmentsumgrad = 0;
            List<StudentGradeViewModel> studentgradeViewmodelList = new List<StudentGradeViewModel>();
            
            foreach(IGrouping<string, Enrollment> group in enrolls)
            {
                Console.WriteLine(group.Key, group.Count());
                foreach(var enrollment in group)
                {
                    enrollmentsumgrad += (int)enrollment.StudentGrade;
                    Enrollname = enrollment.Name;
                }
                var enrollmentaverage = enrollmentsumgrad / group.Count();
                studentgradeViewmodelList.Add(new StudentGradeViewModel() { CourseName = Enrollname, Grade = ((Enrollment.Grade)enrollmentaverage).ToString() });
                enrollmentsumgrad = 0;
            }
            return studentgradeViewmodelList;
            

        }
    }

    
}

