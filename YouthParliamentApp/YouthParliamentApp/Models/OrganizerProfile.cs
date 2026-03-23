using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouthParliamentApp.Models
{
    [Table("OrganizerProfiles")]
    public class OrganizerProfile
    {
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal TrustRating { get; set; } = 5.00m;
        public string? Bio { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
