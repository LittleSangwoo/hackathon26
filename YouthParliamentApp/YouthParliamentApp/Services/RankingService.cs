using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Data;

namespace YouthParliamentDB.Services
{
    public class RankingService
    {
        private readonly ApplicationDbContext _context;

        public RankingService(ApplicationDbContext context) => _context = context;

        public async Task UpdateUserTotalRating(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Participations)
                .ThenInclude(p => p.Event)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                // Алгоритм: Сумма (Баллы * Коэффициент сложности мероприятия)
                user.TotalRating = user.Participations
                    .Where(p => p.Status == 3) // Только для статуса "Завершено/Подтверждено"
                    .Sum(p => p.FinalPoints * p.Event.ComplexityWeight);

                await _context.SaveChangesAsync();
            }
        }
    }
}