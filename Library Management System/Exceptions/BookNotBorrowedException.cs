using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Exceptions
{
    public class BookNotBorrowedException:Exception
    {
        public BookNotBorrowedException(string message):base(message)
        {

        }
    }
}
