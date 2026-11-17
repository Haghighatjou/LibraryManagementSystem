using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class UpdateUserRoleDTO
    {
        //public int Id { get; set; }
        //public string Username { get; set; }

        [Required]
        [RegularExpression("^(User|Admin)$",ErrorMessage = "Role must be User or Admin.")]
        public string Role { get; set; }
    }
}
