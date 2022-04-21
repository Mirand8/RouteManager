using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteManager.Models
{
    public class TeamViewModel
    {
        [NotMapped]
        public string Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "Estado")]
        public string State { get; set; }
        [Display(Name = "Disponibilidade")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Membros")]
        [NotMapped]
        public virtual List<PersonViewModel> Members { get; set; } = new List<PersonViewModel>();
    }
}
