using Microsoft.EntityFrameworkCore;

public class CommunicatorDbContext : DbContext
{
    public CommunicatorDbContext(DbContextOptions<CommunicatorDbContext> options) : base(options)
    { }

    public DbSet<User> User => Set<User>();
    public DbSet<PrivateMessage> PrivateMessage => Set<PrivateMessage>();
    public DbSet<UsersRelation> UsersRelation => Set<UsersRelation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrivateMessage>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.OutcomingPrivateMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PrivateMessage>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.IncomingPrivateMessages)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsersRelation>()
            .HasOne(m => m.SubjectUser)
            .WithMany(u => u.OutcomingUsersRelation)
            .HasForeignKey(m => m.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsersRelation>()
            .HasOne(m => m.TargetUser)
            .WithMany(u => u.IncomingUsersRelation)
            .HasForeignKey(m => m.TargetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}