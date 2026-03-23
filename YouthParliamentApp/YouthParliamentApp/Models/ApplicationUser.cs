using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouthParliamentApp.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required, MaxLength(256)]
        public string FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public int? Age { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalRating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связи
        public virtual OrganizerProfile? OrganizerProfile { get; set; }
        public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
    }
}
