using AIChat1.DTOs;
using AIChat1.Entity;

namespace AIChat1.Helpers
{
    public static class Mapper
    {
        public static ConversationDto ToDto(this Conversation c) => new ConversationDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Title = c.Title,
            CreatedAt = c.CreatedAt,
            ExpiresAt = c.ExpiresAt
        };

        public static MessageDto ToDto(this Message m) =>
            new MessageDto(
                m.Id,
                m.ConversationId,
                m.UserId,
                m.User?.Username ?? string.Empty,
                m.Sender,
                m.Content,
                m.SentAt
            );

        public static UserDto ToDto(this User u) => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
        };

        public static FileDto ToDto(this FileAttachment f) => new FileDto
        {
            Id = f.Id,
            Filename = f.Filename,
            Url = f.Url
        };

        // ===== Inbound (only if needed) =====
        public static Conversation ToEntity(this ConversationDto d) => new Conversation
        {
            Id = d.Id,
            UserId = d.UserId,
            Title = d.Title,
            CreatedAt = d.CreatedAt,
            ExpiresAt = d.ExpiresAt
        };

        public static Message ToEntity(this MessageDto d) => new Message
        {
            Id = d.Id,
            ConversationId = d.ConversationId,
            UserId = d.UserId,
            Sender = d.Sender,
            Content = d.Content,
            SentAt = d.SentAt
        };

        public static FileAttachment ToEntity(this FileDto d) => new FileAttachment
        {
            Id = d.Id,
            Filename = d.Filename,
            Url = d.Url
        };

        // Registration request → User entity
        public static User ToNewUser(this RegisterRequest r, string hashedPassword) => new User
        {
            Username = r.Username, 
            HashedPassword = hashedPassword
        };
    
}
}
