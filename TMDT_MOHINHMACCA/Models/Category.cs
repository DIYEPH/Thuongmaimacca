using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class Category
{
    public int CateId { get; set; }

    public string? CateName { get; set; }

    public string? CateImage { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
