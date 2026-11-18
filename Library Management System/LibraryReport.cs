using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    public class LibraryReport
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int DisabledMembers { get; set; }
    }
}
