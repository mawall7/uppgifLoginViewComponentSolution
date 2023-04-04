using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Models
{
    public class Assignment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public byte[] AssignmentFile { get; set; }

        public int? StudentID { get; set; }
        string StudentName { get; set; }
        DateTime Date { get; set; }



    }

}
