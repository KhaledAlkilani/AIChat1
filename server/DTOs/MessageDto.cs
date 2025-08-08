using AIChat1.Entity;
using System.ComponentModel.DataAnnotations;

namespace AIChat1.DTOs
{
    public class MessageDto : BaseEntity
    {
        public int UserId { get; set; }
        public UserDto Username { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}
