namespace TMDT_MOHINHMACCA.ViewModels
{
    public class UserMessagerVM
    {
        public string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        public string? Lastmess {  get; set; }
        public DateTime? Lastdate {  get; set; }
        public string? Device { get; set; }
        public int Unread { get; set; }
        public bool Online { get; set; }
    }
}
