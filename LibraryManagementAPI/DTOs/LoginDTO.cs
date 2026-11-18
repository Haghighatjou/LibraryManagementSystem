using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(100)]
        public string Username  { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
