using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RouteManager.Models
{
    public class Route
    {
        public List<string> Routes { get; set; } = new List<string>();
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "Serviço")]
        public string Service { get; set; }
    }
}