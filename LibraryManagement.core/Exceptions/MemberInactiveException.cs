using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class MemberInactiveException:Exception
    {
        public MemberInactiveException(string message):base(message)
        {

        }
    }
}
