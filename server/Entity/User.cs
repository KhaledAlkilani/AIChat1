using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIChat1.Entity
{
    public class User : BaseEntity
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string HashedPassword { get; set; } = null!;
    }
}
