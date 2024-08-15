using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderTime { get; set; }

    public int? PostId { get; set; }

    public string? Buyer { get; set; }

    public double? Price { get; set; }

    public string? Status { get; set; }

    public string? Requirements { get; set; }

    public int? Numberday { get; set; }

    public double? Star { get; set; }

    public string? Review { get; set; }

    public string? Link { get; set; }

    public virtual Account? BuyerNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Post? Post { get; set; }
}
