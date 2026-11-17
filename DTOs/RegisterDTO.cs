using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
