using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class OrderDetail
{
    public int DetailId { get; set; }

    public int? OrderId { get; set; }

    public DateTime? Stamptime { get; set; }

    public string? Person { get; set; }

    public string? RequiType { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Account? PersonNavigation { get; set; }
}
