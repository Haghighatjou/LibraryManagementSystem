using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class UserUnavailableException:Exception
    {
        public UserUnavailableException(string message):base(message)
        {

        }
    }
}
