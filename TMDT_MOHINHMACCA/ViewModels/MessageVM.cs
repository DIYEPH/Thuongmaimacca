namespace TMDT_MOHINHMACCA.ViewModels
{
    public class MessageVM
    {
        public int MessageId { get; set; }
        public string? Content { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Avatarsender { get; set; }
        public int? Status {  get; set; }
        public DateTime? Senttime { get; set; }
    }
}
