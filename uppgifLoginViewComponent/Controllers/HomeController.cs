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

namespace uppgifLoginViewComponent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SchoolContext _context;
        private IWebHostEnvironment hostEnv;
        public HomeController(ILogger<HomeController> logger, SchoolContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            hostEnv = env;
            _logger.LogInformation("Logging from Index - index reached");
        }


        private bool HttpOnly { get; set; }

        Dictionary<string, string> KVDict;

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Submit = false;
            IndexViewModel model = new IndexViewModel();

            model.Students = await _context.Students.Include(a => a.Enrollments)
                .Include(a => a.Assignments)
                .ToListAsync();


            return View(model);
        }

        

        



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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, int id) //to do skicka istället en vymodel via en form. IForm som property i vymodellen
        {
            int StudentId = id;
            
           if (isTextFile(file)){
           https://www.msn.com/sv-se/feed
           var filePath = CreateFilePath(file);

           using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                //try
                //{
                    //if (memoryStream.Length < 100000) //validera bytelängd 
                    //{
                        var assignment = new Assignment() { AssignmentFile = memoryStream.ToArray(), Name = file.FileName, Date = DateTime.Now, StudentID = id};//IFORM fil konverteras till bytes eller sparas som textfil.
                        _context.Assignments.Add(assignment);
                        _context.SaveChanges();

                    //}
              }
                //catch(Exception e)
                //{
                //    throw new FileLoadException("file is to large must be less than 10 kb ", e.Message);
                //}
                
            //} 
            
            //using (FileStream fs = System.IO.File.Create(filePath))  //sparar filerna till root/files 
            //{

            //    file.CopyTo(fs);

            //}
            }
            else
            {
                throw new IOException("file must be a text file");

            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult OnSubmit(int studentid)
        {
            ViewBag.Name = _context.Students.Find(studentid).FirstMidName;
            ViewBag.Submit = true;
            return View("Testajax");
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
            var filename = file.FileName;
            filePath = Path.Combine(filePath, filename); //skapa filnamn concatenerarar till en string 
            return filePath;
        }

        public bool isTextFile(IFormFile file)
        {

            return file.ContentType == "text/plain";

        }
    }
}
