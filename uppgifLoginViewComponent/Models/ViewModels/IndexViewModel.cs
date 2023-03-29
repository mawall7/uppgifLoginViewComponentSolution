using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public Student Student { get; set; }
        public LoginViewModel Login { get; set; }

    }

}