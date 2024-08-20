namespace TMDT_MOHINHMACCA.Models;

public partial class Choose
{
    public int ChooseId { get; set; }

    public string? ChooseName { get; set; }

    public int? ChooseTime { get; set; }

    public string? Description { get; set; }

    public int? Discount { get; set; }

    public int? Price { get; set; }

    public string? Rate { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
