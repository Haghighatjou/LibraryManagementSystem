

namespace LibraryManagement.Core.Exceptions
{
    public class BookHasLoanHistoryException: Exception
    {
        public BookHasLoanHistoryException(string message): base(message)
        {

        }
    }
}
