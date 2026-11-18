using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Models
{
    public class LoanReport
    {
        public string BookTitle { get; set; }
        public string MemberName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
