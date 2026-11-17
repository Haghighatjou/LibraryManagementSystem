using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class UpdateMemberDTO
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string? MemberName { get; set; }

        [RegularExpression(@"^09\d{9}$",ErrorMessage ="Phone number must be a valid Iranian mobile number.")]
        public string? PhoneNumber { get; set; }
    }
}
