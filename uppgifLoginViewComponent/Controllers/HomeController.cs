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
        }


        private bool HttpOnly { get; set; }
        
        Dictionary<string, string> KVDict;
       
        public IActionResult Index()
        {

            IndexViewModel model = new IndexViewModel();

            var students = _context.Students.Include(a => a.Enrollments);
            model.Students = students;
            
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




        public ActionResult<CookieViewModel> CreateCookie()
        {
            string key = "cookie_test1";
            string value = DateTime.Now.ToString();
            CookieOptions c = new CookieOptions();
            c.Expires = DateTime.Now.AddMinutes(1);
            c.HttpOnly = !IsInDevelopment(Data.StartupData.MyWebHostEnv); // inga client side scripts kan komma åt cookien (döljs från javascriptet). sätts bara till true när appen är deployed kan annars ge error.
            c.Secure = true;
            c.SameSite = SameSiteMode.Strict;
            Response.Cookies.Append(key, value, c);
            
            CookieViewModel model = new CookieViewModel 
            {
                CookieExpirationDate = c.Expires.ToString()
            };

            return View(nameof(Index), model);

        }
        
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
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string ext = file.ContentType;
            
            if(file.ContentType!="text/plain")
            {
                throw new IOException("file must be a text file");
            }

            var fileDic = "files";
            string filePath = Path.Combine(hostEnv.WebRootPath, fileDic);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var filename = file.FileName;
            filePath = Path.Combine(filePath, filename);
            using(FileStream fs= System.IO.File.Create(filePath))
            {
                file.CopyTo(fs);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
