using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class LoanNotFoundException:Exception
    {
        public LoanNotFoundException(string message) : base(message)
        {

        }
    }
}
