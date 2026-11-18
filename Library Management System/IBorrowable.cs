using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    internal interface IBorrowable
    {
        void Borrow(List<BorrowRecord> borrowRecords);
        void Return(List<BorrowRecord> borrowRecords);
    }
}
