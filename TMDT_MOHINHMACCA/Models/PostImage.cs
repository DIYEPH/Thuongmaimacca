using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class PostImage
{
    public int PostId { get; set; }

    public int ImageId { get; set; }

    public string? Image { get; set; }

    public virtual Post Post { get; set; } = null!;
}
