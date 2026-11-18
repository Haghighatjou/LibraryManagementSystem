using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class MemberNotFoundException:Exception
    {
        public MemberNotFoundException(string message):base(message) 
        {
        } 
    }
}
