using Microsoft.EntityFrameworkCore;

public class CommunicatorDbContext : DbContext
{
    public CommunicatorDbContext(DbContextOptions<CommunicatorDbContext> options) : base(options)
    { }

    public DbSet<User> Users => Set<User>();
    public DbSet<PrivateMessage> PrivateMessages => Set<PrivateMessage>();
    public DbSet<UsersRelation> UsersRelations => Set<UsersRelation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.IncomingPrivateMessages)
            .WithOne(m => m.Recipient)
            .HasForeignKey(m => m.RecipientId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OutcomingPrivateMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.IncomingUsersRelation)
            .WithOne(r => r.TargetUser)
            .HasForeignKey(r => r.TargetId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OutcomingUsersRelation)
            .WithOne(r => r.SubjectUser)
            .HasForeignKey(r => r.SubjectId);
    }
}