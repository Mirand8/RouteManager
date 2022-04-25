using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteManager.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
        }

        public PersonViewModel(string id, string name, string currentTeam)
        {
            Id = id;
            Name = name;
            CurrentTeam = currentTeam;
        }

        [NotMapped]
        public string Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Time Atual")]
        public string CurrentTeam { get; set; }
    }
}
