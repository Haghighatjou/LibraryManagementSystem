using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Exceptions
{
    public class LastAdminCannotBeDemotedException:Exception
    {
        public LastAdminCannotBeDemotedException():base("The last Admin cannot be demoted.")
        {

        }
    }
}
