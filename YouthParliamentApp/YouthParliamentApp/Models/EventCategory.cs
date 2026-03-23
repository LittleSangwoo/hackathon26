using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace YouthParliamentApp.Models
{
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IconUrl { get; set; }

        // Навигационное свойство
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
