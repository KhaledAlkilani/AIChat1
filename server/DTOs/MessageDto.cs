using AIChat1.Entity;
using AIChat1.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIChat1.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public MessageSender Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        public MessageDto(int id, int conversationId, int userId, string username, MessageSender sender, string content, DateTime sentAt)
        {
            Id = id;
            ConversationId = conversationId;
            UserId = userId;
            Username = username;
            Sender = sender;
            Content = content;
            SentAt = sentAt;
        }
    }
}
