using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class BookNotAvailableException:Exception
    {
        public BookNotAvailableException(string message): base(message)
        {

        }
    }
}
