using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models;

namespace uppgifLoginViewComponent.CusomAttributes
{
    public class ValidationFilterFileNotEmptyAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.ActionArguments.SingleOrDefault(p => p.Value is IFormFile);
            var file = (IFormFile)param.Value;
            if(file.Length == 0 )
            {
                context.Result = new BadRequestObjectResult("file was empty");
                return;
            }
           
        }
    }
}
