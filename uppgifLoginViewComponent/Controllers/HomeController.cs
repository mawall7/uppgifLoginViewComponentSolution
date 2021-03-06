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


namespace uppgifLoginViewComponent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        private bool HttpOnly { get; set; }
        public IActionResult Index()
        {
                        
            return View();
        }

        public ActionResult<CookieViewModel> CreateCookie()
        {
            string key = "cookie_test1";
            string value = DateTime.Now.ToString();
            CookieOptions c = new CookieOptions();
            c.Expires = DateTime.Now.AddMinutes(1);
            c.HttpOnly = !IsInDevelopment(Data.StartupData.MyWebHostEnv);
            c.Secure = true;
            c.SameSite = SameSiteMode.Strict;
            Response.Cookies.Append(key, value, c);
            
            CookieViewModel model = new CookieViewModel 
            {
                CookieExpirationDate = c.Expires.ToString()
            };

            return View(nameof(Index), model);

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
    }
}
