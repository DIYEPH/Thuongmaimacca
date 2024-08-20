using Microsoft.EntityFrameworkCore;

namespace TMDT_MOHINHMACCA.Models;

public partial class ShopmaccaContext : DbContext
{
    public ShopmaccaContext()
    {
    }

    public ShopmaccaContext(DbContextOptions<ShopmaccaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Choose> Chooses { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostImage> PostImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transactionhistory> Transactionhistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__ACCOUNT__B15BE12F3966D95C");

            entity.ToTable("ACCOUNT");

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
            entity.Property(e => e.Address).HasColumnName("ADDRESS");
            entity.Property(e => e.Avatar)
                .IsUnicode(false)
                .HasColumnName("AVATAR");
            entity.Property(e => e.Birthday).HasColumnName("BIRTHDAY");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fullname)
                .HasMaxLength(200)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GENDER");
            entity.Property(e => e.GoogleId)
                .IsUnicode(false)
                .HasColumnName("GOOGLE_ID");
            entity.Property(e => e.Money)
                .HasColumnType("money")
                .HasColumnName("MONEY");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Randomkey)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RANDOMKEY");
            entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            entity.Property(e => e.Signupdate)
                .HasColumnType("datetime")
                .HasColumnName("SIGNUPDATE");
            entity.Property(e => e.Validity).HasColumnName("VALIDITY");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ACCOUNT__ROLE_ID__2A4B4B5E");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("PK__CATEGORI__EB0A758F05F8D0A4");

            entity.ToTable("CATEGORIES");

            entity.Property(e => e.CateId).HasColumnName("CATE_ID");
            entity.Property(e => e.CateImage)
                .IsUnicode(false)
                .HasColumnName("CATE_IMAGE");
            entity.Property(e => e.CateName).HasColumnName("CATE_NAME");
        });

        modelBuilder.Entity<Choose>(entity =>
        {
            entity.HasKey(e => e.ChooseId).HasName("PK__CHOOSES__80E419B8A7582BF5");

            entity.ToTable("CHOOSES");

            entity.Property(e => e.ChooseId).HasColumnName("CHOOSE_ID");
            entity.Property(e => e.ChooseName)
                .HasMaxLength(100)
                .HasColumnName("CHOOSE_NAME");
            entity.Property(e => e.ChooseTime).HasColumnName("CHOOSE_TIME");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Discount).HasColumnName("DISCOUNT");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Rate)
                .HasMaxLength(100)
                .HasColumnName("RATE");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__MESSAGE__F610EE443884DFA2");

            entity.ToTable("MESSAGE");

            entity.Property(e => e.MessageId).HasColumnName("MESSAGE_ID");
            entity.Property(e => e.Content).HasColumnName("CONTENT");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RECEIVER_ID");
            entity.Property(e => e.SenderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SENDER_ID");
            entity.Property(e => e.Senttime)
                .HasColumnType("datetime")
                .HasColumnName("SENTTIME");
            entity.Property(e => e.Status).HasColumnName("STATUS");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__MESSAGE__RECEIVE__3D5E1FD2");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__MESSAGE__SENDER___3C69FB99");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__ORDERS__460A9464DC95DBCB");

            entity.ToTable("ORDERS");

            entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");
            entity.Property(e => e.Buyer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BUYER");
            entity.Property(e => e.Link).HasColumnName("LINK");
            entity.Property(e => e.Numberday).HasColumnName("NUMBERDAY");
            entity.Property(e => e.OrderTime)
                .HasColumnType("datetime")
                .HasColumnName("ORDER_TIME");
            entity.Property(e => e.PostId).HasColumnName("POST_ID");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Requirements).HasColumnName("REQUIREMENTS");
            entity.Property(e => e.Review).HasColumnName("REVIEW");
            entity.Property(e => e.Star).HasColumnName("STAR");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("STATUS");

            entity.HasOne(d => d.BuyerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Buyer)
                .HasConstraintName("FK__ORDERS__BUYER__35BCFE0A");

            entity.HasOne(d => d.Post).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ORDERS__POST_ID__34C8D9D1");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__ORDER_DE__76047C44793DC5B8");

            entity.ToTable("ORDER_DETAIL");

            entity.Property(e => e.DetailId).HasColumnName("DETAIL_ID");
            entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");
            entity.Property(e => e.Person)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PERSON");
            entity.Property(e => e.RequiType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("REQUI_TYPE");
            entity.Property(e => e.Stamptime)
                .HasColumnType("datetime")
                .HasColumnName("STAMPTIME");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__ORDER_DET__ORDER__38996AB5");

            entity.HasOne(d => d.PersonNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Person)
                .HasConstraintName("FK__ORDER_DET__PERSO__398D8EEE");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__POST__63FD1766EF102CCF");

            entity.ToTable("POST");

            entity.Property(e => e.PostId).HasColumnName("POST_ID");
            entity.Property(e => e.CateId).HasColumnName("CATE_ID");
            entity.Property(e => e.ChooseId).HasColumnName("CHOOSE_ID");
            entity.Property(e => e.Content).HasColumnName("CONTENT");
            entity.Property(e => e.CoverImage)
                .IsUnicode(false)
                .HasColumnName("COVER_IMAGE");
            entity.Property(e => e.Note).HasColumnName("NOTE");
            entity.Property(e => e.PostApprovedtime)
                .HasColumnType("datetime")
                .HasColumnName("POST_APPROVEDTIME");
            entity.Property(e => e.PostTime)
                .HasColumnType("datetime")
                .HasColumnName("POST_TIME");
            entity.Property(e => e.PriceTo).HasColumnName("PRICE_TO");
            entity.Property(e => e.PriceUp).HasColumnName("PRICE_UP");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("STATUS");
            entity.Property(e => e.Technology).HasColumnName("TECHNOLOGY");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("TITLE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Cate).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__POST__CATE_ID__2D27B809");

            entity.HasOne(d => d.Choose).WithMany(p => p.Posts)
                .HasForeignKey(d => d.ChooseId)
                .HasConstraintName("FK__POST__CHOOSE_ID__2F10007B");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__POST__USERNAME__2E1BDC42");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.ImageId }).HasName("PK__POST_IMA__E4178F0E9C5E2952");

            entity.ToTable("POST_IMAGES");

            entity.Property(e => e.PostId).HasColumnName("POST_ID");
            entity.Property(e => e.ImageId)
                .ValueGeneratedOnAdd()
                .HasColumnName("IMAGE_ID");
            entity.Property(e => e.Image)
                .IsUnicode(false)
                .HasColumnName("IMAGE");

            entity.HasOne(d => d.Post).WithMany(p => p.PostImages)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__POST_IMAG__POST___31EC6D26");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLES__5AC4D222B938A979");

            entity.ToTable("ROLES");

            entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<Transactionhistory>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__TRANSACT__16998B616BBE39A3");

            entity.ToTable("TRANSACTIONHISTORY");

            entity.Property(e => e.TransactionId).HasColumnName("TRANSACTION_ID");
            entity.Property(e => e.Amountmoney)
                .HasColumnType("money")
                .HasColumnName("AMOUNTMONEY");
            entity.Property(e => e.Content).HasColumnName("CONTENT");
            entity.Property(e => e.Finalbalance)
                .HasColumnType("money")
                .HasColumnName("FINALBALANCE");
            entity.Property(e => e.Initialbalance)
                .HasColumnType("money")
                .HasColumnName("INITIALBALANCE");
            entity.Property(e => e.Payments)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PAYMENTS");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("TRANSACTION_DATE");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TRANSACTION_TYPE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Transactionhistories)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("FK__TRANSACTI__USERN__5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
