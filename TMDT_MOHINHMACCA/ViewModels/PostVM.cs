using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.ViewModels
{
    public class PostVM
    {
        public int PostId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? CoverImage { get; set; }

        public int? CateId { get; set; }

        public string? Username { get; set; }

        public string? Technology { get; set; }

        public DateTime? PostTime { get; set; }

        public double? PriceUp { get; set; }

        public double? PriceTo { get; set; }

        public int? ChooseId { get; set; }

        public string? Status { get; set; }
        public double? Star { get; set; }
        public int numberReview {  get; set; }
        public int numberOrder { get; set; }
        public int numberCmt { get; set; }
        public DateTime? PostApprovedtime { get; set; }

        public string? Note { get; set; }
        public virtual Category? Cate { get; set; }

        public virtual Choose? Choose { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

        public virtual Account? UsernameNavigation { get; set; }
    }
}
