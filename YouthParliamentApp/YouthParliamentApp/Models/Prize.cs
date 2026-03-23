using System.ComponentModel.DataAnnotations;

namespace YouthParliamentApp.Models
{
    public class Prize
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public int PointsValue { get; set; }
        public bool IsBonus { get; set; }

        public virtual Event Event { get; set; }
    }
}
