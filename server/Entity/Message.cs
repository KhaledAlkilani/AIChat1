using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIChat1.Entity
{
    public class Message : BaseEntity
    { 
        public int UserId { get; set; }
        public User Username { get; set; } = null!;

        [Required]
        public string Content { get; set; } = "";

        [Required]
        public DateTime SentAt { get; set; }
    }
}