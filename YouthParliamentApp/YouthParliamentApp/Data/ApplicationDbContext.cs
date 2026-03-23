using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Models;

namespace YouthParliamentApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Объявляем наши таблицы (Identity таблицы создаются автоматически под капотом)
        public DbSet<OrganizerProfile> OrganizerProfiles { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Prize> Prizes { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<OrganizerReview> OrganizerReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Обязательная строка при использовании Identity! Без нее будет ошибка при генерации БД.
            base.OnModelCreating(builder);

            // ==========================================
            // НАСТРОЙКА СВЯЗЕЙ (Fluent API)
            // ==========================================

            // 1. Связь 1-к-1: Пользователь <-> Профиль организатора
            builder.Entity<OrganizerProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.OrganizerProfile)
                .HasForeignKey<OrganizerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Если удаляем юзера, удаляется и его профиль

            // 2. Связь Организатор <-> Мероприятия
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление, чтобы ивенты не исчезли случайно

            // 3. Участие в мероприятиях (Participations)
            builder.Entity<Participation>()
                .HasOne(p => p.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Предотвращаем конфликты путей удаления

            builder.Entity<Participation>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participations)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Если удалили ивент, записи об участии в нем тоже удаляются

            // 4. Отзывы об организаторах (OrganizerReviews)
            // Здесь ОБЯЗАТЕЛЬНО DeleteBehavior.Restrict, так как таблица ссылается на Users дважды
            builder.Entity<OrganizerReview>()
                .HasOne(r => r.Organizer)
                .WithMany()
                .HasForeignKey(r => r.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrganizerReview>()
                .HasOne(r => r.Participant)
                .WithMany()
                .HasForeignKey(r => r.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
