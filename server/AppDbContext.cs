using AIChat1.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AIChat1
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Conversation ↔ User(a conversation belongs to a user)
            b.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message ↔ User (a message is authored by one user)
            b.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message ↔ Conversation (one Conversation has many Messages)
            b.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Message>()
             .Property(m => m.Sender)
             .HasConversion<int>(); // enum as int

            b.Entity<FileAttachment>()
                .HasOne(f => f.Conversation)
                .WithMany()
                .HasForeignKey(f => f.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
};