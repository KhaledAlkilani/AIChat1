using AIChat1.Entity.Enums;

namespace AIChat1.DTOs
{
    public class ChatDto
    {
        public record ConversationDto(int Id, int UserId, DateTime CreatedAt);
        public record CreateConversationRequest(int UserId);
        public record SendMessageRequest(int UserId, int ConversationId, string Content);
        public record MessageDto(int Id, int ConversationId, int UserId, MessageSender Sender, string Content, DateTime SentAt, string Username);
    }
}
