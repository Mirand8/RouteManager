using System.ComponentModel.DataAnnotations;

namespace RouteManager.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email invalido!")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
