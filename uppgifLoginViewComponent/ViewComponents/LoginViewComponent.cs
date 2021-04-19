using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models.ViewModels;

namespace uppgifLoginViewComponent.ViewComponents
{
    public class LoginViewComponent:ViewComponent
    {
       
        public LoginViewModel Model { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            try
            {
                Model = new LoginViewModel { IsLoggedIn = User.Identity.IsAuthenticated, Name = User.Identity.Name };

            }
            catch (ArgumentNullException) { }

            return View(Model);

        }
    }
}
