using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Exceptions
{
    public class BookNotFoundException:Exception
    {
        public BookNotFoundException(string message):base(message)
        {

        }
    }
}
