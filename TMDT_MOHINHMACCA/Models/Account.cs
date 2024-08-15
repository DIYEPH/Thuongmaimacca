using System;
using System.Collections.Generic;

namespace TMDT_MOHINHMACCA.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string? Fullname { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal? Money { get; set; }

    public int? RoleId { get; set; }

    public DateTime? Signupdate { get; set; }

    public bool? Validity { get; set; }

    public string? Randomkey { get; set; }

    public string? GoogleId { get; set; }

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transactionhistory> Transactionhistories { get; set; } = new List<Transactionhistory>();
}
