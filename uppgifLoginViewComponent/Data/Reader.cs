using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uppgifLoginViewComponent.Data
{
    public class Reader
    {
        public string Read(byte[] bytes)
        {
            string s = Encoding.Default.GetString(bytes);
            //StringBuilder builder = new StringBuilder();
            //for(int i = 0; i < bytes.Length; i++)
            //{
            //    builder.Append(bytes[i].ToString("x2"));
            //}
            //String s = builder.ToString();
            return s;
        }
    }
}
