namespace TMDT_MOHINHMACCA.ViewModels
{
    public class ProfileVM
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password_new { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Gender { get; set; }
        public string Avatar { get; set; }
    }
}
