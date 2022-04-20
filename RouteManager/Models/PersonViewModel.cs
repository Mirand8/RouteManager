using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteManager.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
        }

        public PersonViewModel(string id, string name, bool isAvailableToTeam)
        {
            Id = id;
            Name = name;
            IsAvailableToTeam = isAvailableToTeam;
        }

        [NotMapped]
        public string Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Disponibilidade Para Time")]
        public bool IsAvailableToTeam { get; set; }
    }
}
