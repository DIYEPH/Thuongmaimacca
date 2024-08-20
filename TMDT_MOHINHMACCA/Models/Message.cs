namespace TMDT_MOHINHMACCA.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public int? Status { get; set; }

    public string? Content { get; set; }

    public DateTime? Senttime { get; set; }

    public virtual Account? Receiver { get; set; }

    public virtual Account? Sender { get; set; }
}
