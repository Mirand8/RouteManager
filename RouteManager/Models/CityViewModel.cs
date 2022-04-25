using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteManager.Models
{
    public class CityViewModel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Name { get; set; }

        [JsonProperty("State")]
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string State { get; set; }

        [NotMapped]
        public string CityRepresentation => Name + " - " + State;
    }
}
