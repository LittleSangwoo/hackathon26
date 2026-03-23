using System.ComponentModel.DataAnnotations.Schema;
using static YouthParliamentApp.Models.Enums;

namespace YouthParliamentApp.Models
{
    public class Participation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? CheckInDate { get; set; }
        public int Status { get; set; } // 0: Registered, 1: Attended, 2: Rejected

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FinalPoints { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Event Event { get; set; }
    }
}
