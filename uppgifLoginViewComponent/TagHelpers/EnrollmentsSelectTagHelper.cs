using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace uppgifLoginViewComponent.TagHelpers
{
    public class EnrollmentsSelectTagHelper : TagHelper
    {
        public List<Enrollment> Selectvalues { get; set; }
        public List<SelectListItem> SelectItems{  get {return ReturnSelectvalues();} set {; } } 
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName ="select";
            output.Attributes.Add("asp-items", SelectItems);
            
            StringBuilder strbuilder = new StringBuilder();
            strbuilder.Append("<option value='0' selected ='selected'>Se pågående kurser</option>");
           
            foreach (var item in SelectItems)
            {
                strbuilder.Append($"<option value = {item.Value}>{item.Text}</option>");


            }
                output.Content.SetHtmlContent(strbuilder.ToString());

            
        }
         //todo returnera en lista och en asp-items/ fungerar ej 
        private List<SelectListItem> ReturnSelectvalues()
        {
            List<SelectListItem> listitems = new List<SelectListItem>();
            foreach(var item in Selectvalues)
            {
                listitems.Add(new SelectListItem() { Text = item.Name,  Value = item.Name });
            }

            return listitems; 
            
        }
    }
}
