using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouthParliamentApp.Models
{
    public class AppModels
    {
    }
    public class ApplicationRole : IdentityRole<Guid>
    {
        // Поле Name уже есть внутри IdentityRole, 
        // поэтому добавлять его отдельно не нужно.
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

    // =========================================
    // 2. ТАБЛИЦА: Users (Пользователи)
    // Наследуется от IdentityUser (Email, PasswordHash уже внутри)
    // =========================================
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required, MaxLength(256)]
        public string FullName { get; set; }

        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public int? Age { get; set; }

        // Прямая связь с ролью, как на твоей диаграмме
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalRating { get; set; } = 0.00m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства (связи с другими таблицами)
        public virtual OrganizerProfile? OrganizerProfile { get; set; }
        public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
        public virtual ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    }

    // =========================================
    // 3. ТАБЛИЦА: OrganizerProfiles (Профили организаторов)
    // =========================================
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

    // =========================================
    // 4. ТАБЛИЦА: EventCategories (Категории мероприятий)
    // =========================================
    public class EventCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? IconUrl { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }

    // =========================================
    // 5. ТАБЛИЦА: Events (Мероприятия)
    // =========================================
    public class Event
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizerId { get; set; }
        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual EventCategory Category { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime EventDate { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal ComplexityWeight { get; set; } = 1.00m;

        public int Status { get; set; } = 0; // 0: Draft, 1: Active, 2: Completed, 3: Cancelled

        [MaxLength(50)]
        public string? VerificationCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связи
        public virtual ICollection<Prize> Prizes { get; set; } = new List<Prize>();
        public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
    }

    // =========================================
    // 6. ТАБЛИЦА: Prizes (Призы)
    // =========================================
    public class Prize
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        public int PointsValue { get; set; } = 0;
        public bool IsBonus { get; set; } = false;
    }

    // =========================================
    // 7. ТАБЛИЦА: Participations (Участие)
    // =========================================
    public class Participation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? CheckInDate { get; set; }

        public int Status { get; set; } = 0; // 0: Registered, 1: Attended, 2: Rejected

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FinalPoints { get; set; } = 0.00m;
    }

    // =========================================
    // 8. ТАБЛИЦА: OrganizerReviews (Отзывы)
    // =========================================
    public class OrganizerReview
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizerId { get; set; }
        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }

        [Required]
        public Guid ParticipantId { get; set; }
        [ForeignKey("ParticipantId")]
        public virtual ApplicationUser Participant { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
        public class LeaderboardItem
        {
            public int Rank { get; set; }
            public string FullName { get; set; }
            public string City { get; set; }
            public decimal TotalRating { get; set; }
        }

        public class LeaderboardViewModel
        {
            public List<LeaderboardItem> TopUsers { get; set; }
            public int? SelectedCategoryId { get; set; }
            public List<EventCategory> Categories { get; set; }
        
    }
}
