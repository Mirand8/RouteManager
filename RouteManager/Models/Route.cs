using System.Collections.Generic;

namespace RouteManager.Models
{
    public class Route
    {
        public List<string> Routes { get; set; } = new List<string>();
        public string City { get; set; }
        public string Service { get; set; }
    }
}