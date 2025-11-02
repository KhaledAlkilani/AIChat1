using AIChat1.Entity;

namespace AIChat1.DTOs
{
    public class ConversationDto : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
