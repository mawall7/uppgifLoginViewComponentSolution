using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models
{
    public class StudentViewModel
    {
        public int ID { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Postnummer")]
        [DataType(DataType.PostalCode)]
        [Required]
        [RegularExpression(@"/d{5}/s/[a-öA-Ö]")]
        public string PostalCode { get; set; }
    }
}
