using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class BookNotFoundException: Exception
    {
        public BookNotFoundException(string message): base(message)
        {

        }
    }
}
