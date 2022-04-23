using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteManager.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
        }

        public PersonViewModel(string id, string name, bool isOnATeam)
        {
            Id = id;
            Name = name;
            IsOnTeam = isOnATeam;
        }

        [NotMapped]
        public string Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Esta em Time")]
        public bool IsOnTeam { get; set; }
    }
}
