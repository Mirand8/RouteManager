using System.ComponentModel.DataAnnotations;

namespace RouteManager.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
