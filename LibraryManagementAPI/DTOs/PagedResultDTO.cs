using LibraryManagement.Core.Models;

namespace LibraryManagementAPI.DTOs
{
    public class PagedResultDTO
    {
        public List<BookDTO> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
