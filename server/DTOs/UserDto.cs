using AIChat1.Entity;
using System.ComponentModel.DataAnnotations;

namespace AIChat1.DTOs
{
    public class UserDto : BaseEntity
    {
        [Required]
        public string Username { get; set; } = "";
    }
}
