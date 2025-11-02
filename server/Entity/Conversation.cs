namespace AIChat1.Entity
{ 
    public class Conversation : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
