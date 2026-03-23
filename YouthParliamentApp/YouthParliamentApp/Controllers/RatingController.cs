using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Data;
using YouthParliamentApp.Models; // Проверь, чтобы namespace совпадал

namespace YouthParliamentApp.Controllers
{
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Основной метод страницы рейтинга
        public async Task<IActionResult> Index(int? categoryId)
        {
            // 1. Начинаем запрос к пользователям
            var query = _context.Users.AsQueryable();

            // 2. Если нажата кнопка категории (IT/Медиа), фильтруем
            if (categoryId.HasValue)
            {
                query = query.Where(u => u.Participations
                    .Any(p => p.Event.CategoryId == categoryId.Value));
            }

            // 3. Берем Топ-100, сортируем и превращаем в наш "легкий" класс
            var users = await query
                .OrderByDescending(u => u.TotalRating)
                .Take(100)
                .Select(u => new LeaderboardItem
                {
                    FullName = u.FullName,
                    City = u.City,
                    TotalRating = u.TotalRating
                })
                .ToListAsync();

            // 4. Проставляем места (1, 2, 3...)
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Rank = i + 1;
            }

            // 5. Собираем всё в одну "коробку" (ViewModel)
            var viewModel = new LeaderboardViewModel
            {
                TopUsers = users,
                SelectedCategoryId = categoryId,
                Categories = await _context.EventCategories.ToListAsync()
            };

            return View(viewModel);
        }
    }
}