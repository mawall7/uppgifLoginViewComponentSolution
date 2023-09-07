using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Models;

namespace uppgifLoginViewComponent.Extensionmethods
{
    public static class ListExtension
    {
        public static bool NameIsDuplicated(this List<Student> list, Student s)
        {
            return list.Any(i => i.FirstMidName == s.FirstMidName);
        }
    }
}
