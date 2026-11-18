using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class MemberAlreadyActiveException:Exception
    {
        public MemberAlreadyActiveException(string message) : base(message)
        {
        }
    }
}
