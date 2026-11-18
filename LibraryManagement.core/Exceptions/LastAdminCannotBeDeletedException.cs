using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class LastAdminCannotBeDeletedException:Exception
    {
        public LastAdminCannotBeDeletedException(string message) : base(message) 
        {
        }
    }
}
