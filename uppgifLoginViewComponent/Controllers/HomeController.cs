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

namespace uppgifLoginViewComponent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SchoolContext _context;
        private IWebHostEnvironment hostEnv;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, SchoolContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            hostEnv = env;
            _logger.LogInformation("Logging from Index - index reached");
            _mapper = mapper;
        }
        private bool HttpOnly { get; set; }
        Dictionary<string, string> KVDict;


        [HttpGet]
        public async Task<IActionResult> Index() //selected student skickas från OnSubmit Action metod nedan i Redirect från selectList formsubmit 
        {
            ViewBag.Submit = false;
            IndexViewModel model = new IndexViewModel();
            var studentselect = new List<SelectListItem>();

            //model.Students = await _context.Students.Include(a => a.Enrollments)
            //    .Include(a => a.Assignments)
            //    .ToListAsync();
            model.Students = await _context.Students.Include(a => a.Enrollments)
            .ToListAsync();
                
            //var Enrollment = await _context.Enrollments.Include(a => a.Assignments).Include(a => a.Student)
            //    .ToListAsync();

            //model.Student = selected;
            //model.Student.Enrollments = await _context.Enrollments.Where(s => s.StudentID == selected.ID).Include(e=>e.Assignments).ToListAsync();
                //_context.Students.Where(s => s.ID == model.StudentId).Include(s => s.Enrollments).FirstOrDefault();
               
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
           // studentselect.Add(new SelectListItem() { Value = 0.ToString(), Text = "--Select Student--", Selected = true });
            model.StudentsSelect = studentselect;
            
            return View(model);
        }
            
        //public bool IsDuplicated(Student s, List<Student> list) 
        //{
        //    return list.Any(i => i.FirstMidName == s.FirstMidName);
        //}
       
            public IActionResult StudentsPartial()
        {
            var st = _context.Students.ToList();
           

            return PartialView("_Students"/*_context.Students.ToList()*/);
            
        }
        public IActionResult ModalPartial()
        {
            return PartialView("_Modal");
        }

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



        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int Id) //to do kolla vilken student som laddat upp t.ex. döp om filen till att sluta med studentnamn via student id
        {
            //var uploads = @"C:\Users\matte\Downloads\";
            int teststudid = Id;
            
            //spara filen
            var uploads = CreateFilePath(file);

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                //sparars fil i databas som binär kod eller blob 

                var mstream = new MemoryStream(); //hur använd using statement?
                file.CopyTo(mstream);
                byte[] byteArray = mstream.ToArray();
                //var assignment = new Assignment() { SubmissionDate = DateTime.Now, Name = file.FileName, StudentID = Id, StudentName = _context.Students.Find(Id).LastName, AssignmentFile = byteArray };
                var assignment = new Assignment() { SubmissionDate = DateTime.Now, Name = file.FileName, CourseAssignmentID=1, CourseID=1, EnrollmentID=1 /*StudentID = Id, StudentName = _context.Students.Find(Id).LastName,*/, AssignmentFile = byteArray };
                _context.Assignments.Add(assignment);
                _context.SaveChanges();
                
                //// to do gör en Db klass istället för att jobba mot contextet direkt: Db.SaveAssignment(name); 
            }

            return RedirectToAction("Index");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////public async Task<IActionResult> Upload(IFormFile file, int id) //to do skicka istället en vymodel via en form. IForm som property i vymodellen
        //{
        //    int StudentId = id;

        //   if (isTextFile(file)){

        //   var filePath = CreateFilePath(file);  verkar inte fungera

        //   using (var memoryStream = new MemoryStream())
        //    {
        //        await file.CopyToAsync(memoryStream);
        //        //try
        //        //{
        //            //if (memoryStream.Length < 100000) //validera bytelängd 
        //            //{
        //                var assignment = new Assignment() { AssignmentFile = memoryStream.ToArray(), Name = file.FileName, Date = DateTime.Now, StudentID = id};//IFORM fil konverteras till bytes eller sparas som textfil.
        //                _context.Assignments.Add(assignment);
        //                _context.SaveChanges();

        //            //}
        //      }
        //        //catch(Exception e)
        //        //{
        //        //    throw new FileLoadException("file is to large must be less than 10 kb ", e.Message);
        //        //}

        //        //} 

        //        //using (FileStream fs = System.IO.File.Create(filePath))  //sparar filerna till root/files 
        //        //{

        //        //    file.CopyTo(fs);

        //        //}
        //    }
        //    else
        //    {
        //        throw new IOException("file must be a text file");

        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public IActionResult OnSubmit(IndexViewModel model)
        {
            Student s;
            try
            {
                //s = _context.Students.Find(model.StudentId);
                s = _context.Students.Where(s => s.ID == model.StudentId).Include(s => s.Enrollments).FirstOrDefault();
               // ViewBag.Submit = true;

                //return View("Testajax");
            }
            catch (Exception e)
            {

                throw e;
            }
            return RedirectToAction(nameof(Index), s);
        }

        [HttpPost]
        //parameter IndexViewModel model
        public IActionResult OnSubmitAjax(int StudentId) //obs att selectlistan selected inte uppdateras görs genom 1) javascript onsubmit eller 2) att ta med selectlistan i partiella vyn "TestAjax".
        {
            //Skapa istället en StudentViewModel och använd fetch istället för ajax som verkar vara avvecklad
            //gör en todo och visa inlämnade uppgifter och inlämnings/ sista inl.datum för sent ska det inte gå att lämna
            Student s;
            try
            {
                s = _context.Students.Where(s => s.ID == StudentId).Include(s => s.Enrollments).ThenInclude(a => a.Assignments).FirstOrDefault();
                //    .Include(s => s.Assignments).FirstOrDefault(); //automapper
                
                
                StudentInfoViewModel model = _mapper.Map<StudentInfoViewModel>(s);
                model.CourseAssignment = _context.CourseAssignment.ToList();
                ViewBag.Submit = true;
               
                return View("Testajax", model);
            }
            catch (Exception e)
            {

                throw e;
            }
            //return RedirectToAction(nameof(Index), s);
        }







        [HttpPost]
        public IActionResult OnStudinfoSubmit(int studentid)
        {
            
            var student = _context.Students.Find(studentid);
         
            ViewBag.Submit = true;
            return View("Studentinfo", student);
            
        }

        private string CreateFilePath(IFormFile file)
        {
            var fileDic = "files";
            string filePath = Path.Combine(hostEnv.WebRootPath, fileDic);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath); //skapar en mapp files under root

            }
            //var filename = file.FileName;
            //filePath = Path.Combine(filePath, filename); //skapa filnamn concatenerarar till en string 
            return filePath;
        }

        public bool isTextFile(IFormFile file)
        {

            return file.ContentType == "text/plain";

        }
    }
}
