using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models.ViewModels
{
    public class StudentInfoViewModel
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Postnummer")]
        [DataType(DataType.PostalCode)]
        [Required(ErrorMessage = "Postadress krävs")]
        [RegularExpression(@"^[1-9]{5}\s[a-zA-Z]{2,15}$")]
        [StringLength(20)]
        public string PostalCode { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
    }
}
