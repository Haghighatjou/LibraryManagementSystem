using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
                
        public string Title { get; set; }
                
        public string Author    { get; set; }
                
        public int PublishYear {  get; set; }
                
    }
}
