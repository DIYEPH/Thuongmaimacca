using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class Transactionhistory
{
    public int TransactionId { get; set; }

    public string? Username { get; set; }

    public string? TransactionType { get; set; }

    public decimal? Amountmoney { get; set; }

    public decimal? Initialbalance { get; set; }

    public decimal? Finalbalance { get; set; }

    public string? Content { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Payments { get; set; }

    public virtual Account? UsernameNavigation { get; set; }
}
