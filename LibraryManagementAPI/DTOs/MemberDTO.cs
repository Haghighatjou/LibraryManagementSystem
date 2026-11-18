namespace LibraryManagementAPI.DTOs
{
    public class MemberDTO
    {
        public int Id { get; set; }
        public string MemberName { get; set; }
        public string PhoneNumber   { get; set; }
        public bool IsActive { get; set; }
    }
}
