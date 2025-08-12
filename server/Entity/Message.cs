using AIChat1.Entity.Enums;
using System.ComponentModel.DataAnnotations; 

namespace AIChat1.Entity
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public MessageSender Sender { get; set; } = MessageSender.User;

        [Required]
        public string Content { get; set; } = "";

        [Required]
        public DateTime SentAt { get; set; }
    }
}