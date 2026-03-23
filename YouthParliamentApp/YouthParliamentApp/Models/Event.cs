using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static YouthParliamentApp.Models.Enums;

namespace YouthParliamentApp.Models
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizerId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime EventDate { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal ComplexityWeight { get; set; } = 1.00m;

        public int Status { get; set; } // Можно заменить на Enum (Draft, Active...)
        public string? VerificationCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связи
        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }
        public virtual EventCategory Category { get; set; }
        public virtual ICollection<Prize> Prizes { get; set; } = new List<Prize>();
        public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
    }
}
