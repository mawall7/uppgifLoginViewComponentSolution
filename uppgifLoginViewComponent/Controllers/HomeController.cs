using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models;
using uppgifLoginViewComponent.Models.ViewModels;
using uppgifLoginViewComponent.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using uppgifLoginViewComponent.Extensionmethods;
using AutoMapper;
using uppgifLoginViewComponent.CusomAttributes;

namespace uppgifLoginViewComponent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SchoolContext _context;
        private IWebHostEnvironment hostEnv;
        private readonly IMapper _mapper;
        private readonly IdentityDbContext _identitycontext;
        public HomeController(ILogger<HomeController> logger, SchoolContext context, IWebHostEnvironment env, IMapper mapper, IdentityDbContext identitycontext)
        {
            _logger = logger;
            _context = context;
            hostEnv = env;
            _logger.LogInformation("Logging from Index - index reached");
            _mapper = mapper;
            _identitycontext = identitycontext;
        }
        private bool HttpOnly { get; set; }
        Dictionary<string, string> KVDict;


        [HttpGet]
        public async Task<IActionResult> Index(string? assignmentupdate) //selected student skickas från OnSubmit Action metod nedan i Redirect från selectList formsubmit 
        {
            ViewBag.Submit = false;
            TempData.Keep("GradechangeInfo");
            TempData.Keep("GradechangeEnrollmentId");

            TempData.Keep("Message");


            IndexViewModel model = new IndexViewModel();
            var studentselect = new List<SelectListItem>();

            model.Students = await _context.Students.Include(a => a.Enrollments)
            .ToListAsync();

            ViewBag.AssignmentUpdated = assignmentupdate;   
            var studentslist = new List<Student>();
            //studentslist = _context.Students.ToList();
            
            foreach (var s in _context.Students) 
            {
               
                if (studentslist.NameIsDuplicated(s)) 
                {
                    continue;
                }
                else
                {
                    studentslist.Add(s);
                    studentselect.Add(new SelectListItem() { Value = s.ID.ToString(), Text = $"{s.FirstMidName} {s.LastName }" });
                }    
            }
          
            model.StudentsSelect = studentselect;
            
            return View(model);
        }

        //från exempel används ej för tillfället
        public IActionResult StudentsPartial()
        {
            var st = _context.Students.ToList();


            return PartialView("_Students"/*_context.Students.ToList()*/);

        }
        
        //från exempel används ej för tillfället
        public IActionResult ModalPartial()
        {
            
            return PartialView("_Modal");
        }


        //från exempel används ej för tillfället
        public IActionResult StudentÉnrollment(int? ID)
        {
            Enrollment enrollment = _context.Enrollments.Where(a => a.StudentID == ID).FirstOrDefault();
            return new JsonResult(enrollment);
            
        }

        public IActionResult getJsonEnrolls(string value)
        {
            //Ok :)
            CourseViewModel enrollment = _context.Courses.Where(a => a.Title == value)
               .Select(a =>

                new CourseViewModel() { Title = a.Title, Credits = a.Credits }
             ).FirstOrDefault();   
            return new JsonResult(enrollment);

        }




        //public ActionResult<CookieViewModel> CreateCookie()
        //{
        //    string key = "cookie_test1";
        //    string value = DateTime.Now.ToString();
        //    CookieOptions c = new CookieOptions();
        //    c.Expires = DateTime.Now.AddMinutes(1);
        //    c.HttpOnly = !IsInDevelopment(Data.StartupData.MyWebHostEnv); // inga client side scripts kan komma åt cookien (döljs från javascriptet). sätts bara till true när appen är deployed kan annars ge error.
        //    c.Secure = true;
        //    c.SameSite = SameSiteMode.Strict;
        //    Response.Cookies.Append(key, value, c);
            
        //    CookieViewModel model = new CookieViewModel 
        //    {
        //        CookieExpirationDate = c.Expires.ToString()
        //    };

        //    return View(nameof(Index), model);

        //}
        [AutoValidateAntiforgeryToken]
        public IActionResult Delete(string courseTitle, string lastname)
        {
            var studentid = _context.Students.Where(a => a.LastName == lastname).Select(s=> s.ID).FirstOrDefault();
            
            var enrollmentslist = _context.Enrollments.Where(a => a.StudentID == studentid).ToList();
            var enrollment = enrollmentslist.Where(a => a.Name == courseTitle).FirstOrDefault();
            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveCookie()
        {           
            Response.Cookies.Delete("cookie_test1");
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public bool IsInDevelopment(IWebHostEnvironment e)
        {
            return !e.IsDevelopment(); 
             
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ServiceFilter(typeof(ValidationFilterFileNotEmptyAttribute))] //skrivet attribut av mig för att kolla om fil innehåller text ej är tom 
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int Id, string CourseName) //to do kolla vilken student som laddat upp t.ex. döp om filen till att sluta med studentnamn via student id
        {
            //var uploads = @"C:\Users\matte\Downloads\";
            //sparas bara lokalt i localdb kan så klart bytas ut
            //till en mySQL moln databas mongo db eller liknande.
            int studentid = Id;
            string courseName = CourseName;
            
            var uploads = CreateFilePath(file);

            if (file.Length > 0)
            {

                //spara fil i localdb som binär kod

                using (var mstream = new MemoryStream())
                { //using?
                    
                    file.CopyTo(mstream);
                    byte[] byteArray = mstream.ToArray(); //alt Encoding.Default.GetBytes(string)

                    Enrollment enrollment = await _context.Enrollments.Where(enrollment =>
                    enrollment.Student.ID == Id && enrollment.Course.Title == CourseName)
                        .FirstOrDefaultAsync(); //await async 
                    //if enrollment not choosen  display message

                    Assignment assignment = new Assignment(){/*ta bort courseassignmentID har redan CourseID !*/
                        //ID ?? skapas dynamiskt här också? ja
                        Name = file.FileName,
                        SubmissionDate = DateTime.Now,
                        EnrollmentID = enrollment.ID,
                        CourseID = enrollment.CourseID,
                        AssignmentFile = byteArray
                    };
               
                await _context.Assignments.AddAsync(assignment);
                await _context.SaveChangesAsync();
                
                }
            }

            return RedirectToAction("Index");
        }
        

        [HttpPost]
        public IActionResult OnSubmit(IndexViewModel model)
        {
            Student s;
            try
            {
             
                s = _context.Students.Where(s => s.ID == model.StudentId).Include(s => s.Enrollments).FirstOrDefault();
               
            }
            catch (Exception e)
            {

                throw e;
            }
            return RedirectToAction(nameof(Index), s);
        }

        [HttpPost]
        public async Task <IActionResult> GetEmailEdit(int id, StudentInfoViewModel student) //varför student null?
        {
            var stud = await _context.Students.Where(s => s.ID == id).FirstOrDefaultAsync();
            return PartialView("_EditStudentEmail", stud);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentEmail(int ID, string email)
        {   

            //Ändra tillbaks senare? ändrar man mail då ändras även useremail som används som password inte säkert att man vill ha den implementationen.
            //Beroende på om en user ska ha authorisering för att ändra password. 
            var student = await _context.Students.Where(s => s.ID == ID).FirstOrDefaultAsync();

            var oldemail = student.Email;
            var user = await _identitycontext.Users.Where(u => u.UserName == oldemail).FirstOrDefaultAsync();
            user.Email = email; user.UserName = email; user.NormalizedUserName = email.ToUpper();user.NormalizedUserName = email; 
            _identitycontext.Users.Update(user);
            await _identitycontext.SaveChangesAsync();

            student.Email = email;
            _context.Update(student);
            await _context.SaveChangesAsync();
              
            ViewBag.emailIsSaved = true; //validering använd helpers istället 

            return PartialView("_Studentemailsaved");
        }

        
        [HttpPost]
        public IActionResult OnSubmitAjax(int StudentId) //obs att selectlistan selected inte uppdateras görs genom 1) javascript onsubmit eller 2) att ta med selectlistan i partiella vyn "TestAjax".
        {
            //To do skapa istället en StudentViewModel och använd fetch istället för ajax som verkar vara avvecklad? < ja och nej. gjorde inte det i alla fall, eftersom det inte är meningen att man ska använda
            //fetch i någon större utsträckning då koden är svårtestad, eftersom det då mer blir javascript baserad kod//verkar ändå som det fortfarnde används
            //även om fetch api också används.  
            //gör en todo och visa inlämnade uppgifter och inlämnings/ sista inl.datum för sent ska det inte gå att lämna
            Student s;
            try
            {
                s = _context.Students.Where(s => s.ID == StudentId).Include(s => s.Enrollments).ThenInclude(a => a.Assignments).FirstOrDefault();

                StudentInfoViewModel model = _mapper.Map<StudentInfoViewModel>(s);
                model.CourseAssignment = _context.CourseAssignment.ToList();
                ViewBag.Submit = true;
               
                return PartialView("Testajax", model);
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        [HttpPost]
        public IActionResult OnStudinfoSubmit(int studentid)
        {
            
            var student = _context.Students.Find(studentid);
         
            ViewBag.Submit = true;
            return View("Studentinfo", student);
            
        }

        public async Task<IActionResult> StudAssignment(int Id, int AssId)//borde använda fullständigt namn
        {
            var assignments = await _context.Enrollments.Include(e => e.Assignments)
                .Where(a=> a.ID == Id).ToListAsync();
            
            var sassignment = assignments.Select(s =>
            new Assignment()
            {
                AssignmentFile = s.Assignments //skicka en viewmodel istället
            .Where(a => a.ID == AssId)
            .Select(s => s.AssignmentFile).FirstOrDefault()
            }
            ).FirstOrDefault();
            

            //
            var assignmentviewmodel = assignments.Select(s =>
            new AssignmentViewModel()
            {

                File = s.Assignments //skicka en viewmodel istället
            .Where(a => a.ID == AssId)
            .Select(s => s.AssignmentFile).FirstOrDefault()
            ,
                CourseIDAssignment = _context.Assignments
                  .Where(assignment =>
                  assignment.ID == AssId)
                  .Select(item => item.CourseID)
                  .FirstOrDefault()
            }).FirstOrDefault();
            //

            ViewBag.CourseAssignmentId = _context.Assignments. //obs använd helst inte då den är hårt typad använd viewmodel istället för det här.
                Where(assignment =>
                assignment.ID == AssId)
                .Select(item => item.CourseID)
                .FirstOrDefault();


            return View(assignmentviewmodel);
        }

        private string CreateFilePath(IFormFile file)
        {
            var fileDic = "files";
            string filePath = Path.Combine(hostEnv.WebRootPath, fileDic);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath); //skapar en mapp files under root

            }
            return filePath;
        }

        public bool IsTextFile(IFormFile file)
        {

            return file.ContentType == "text/plain";

        }

        [ValidateAntiForgeryToken]
#nullable enable
        public async Task<IActionResult> GradeAssignment(string Grade) 
        {
            var gradecourse = Grade.Split(",");
            var grade = gradecourse[0];
            var courseid = gradecourse[1];
            int assignmentCourse = int.Parse(courseid);
            Enrollment enrollment = await _context.Enrollments
            .Where(enrollment => enrollment.CourseID == assignmentCourse)
            .FirstOrDefaultAsync();
            enrollment.StudentGrade = Enum.Parse<Enrollment.Grade>(grade);
            _context.Update(enrollment);
            _context.SaveChanges();
            string info = "updated";

            TempData["GradechangeInfo"] = info;
            TempData["GradechangeEnrollmentId"] = enrollment.ID.ToString();
            
            TempData.Keep("GradechangeStatus");
            

            return RedirectToAction("Index", "Home", new { assignmentupdate = "updated" });
          
        }

        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            var assignmentfile = await _context.Assignments.Where(item => item.ID == assignmentId)
                .FirstOrDefaultAsync();

            return View(nameof(DeleteAssignmentConfirm), new DeleteFileViewModel(){FileName = assignmentfile.Name, FileId = assignmentfile.ID });
        }
        public async Task<IActionResult> DeleteAssignmentConfirm(int assignmentId)
        {
            var assignmentfile = await _context.Assignments.Where(item => item.ID == assignmentId)
                .FirstOrDefaultAsync();
            _context.Remove(assignmentfile);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Successfully Removed a File";
            
            return RedirectToAction(nameof(Index));
        }

    }
}
