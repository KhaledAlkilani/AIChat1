using System.ComponentModel.DataAnnotations; 

namespace AIChat1.Entity
{ 
    public class FileAttachment : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        [Required] public string Filename { get; set; } = "";
        [Required] public string FileType { get; set; } = "";
        [Required] public string Url { get; set; } = "";
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }
}
