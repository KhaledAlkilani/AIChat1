using System.ComponentModel.DataAnnotations;

namespace AIChat1.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = null!;
    }
}
