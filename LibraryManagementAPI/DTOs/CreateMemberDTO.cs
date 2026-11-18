using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class CreateMemberDTO
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string MemberName { get; set; }

        [Required]
        [RegularExpression(@"^09\d{9}$", ErrorMessage ="Phone Number must be a valid Iranian mobile number.")]
        public string PhoneNumber { get; set; }
    }
}
