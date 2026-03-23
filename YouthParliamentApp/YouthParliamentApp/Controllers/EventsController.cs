using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouthParliamentApp.Data;
using YouthParliamentApp.Models;
using YouthParliamentApp.ViewModels;

namespace YouthParliamentApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 1. КАРТОЧКА МЕРОПРИЯТИЯ (Доступно всем)
        // ==========================================
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Prizes)
                .Include(e => e.Participations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();
            return View(ev);
        }

        // ==========================================
        // 2. СОЗДАНИЕ МЕРОПРИЯТИЯ + ПРИЗЫ (Только Организатор)
        // ==========================================
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            // Здесь передаем категории во View (через ViewBag)
            ViewBag.Categories = _context.EventCategories.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> ScanQR(Guid eventId, string secretCode)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));

            // Ищем заявку пользователя на это мероприятие
            var participation = await _context.Participations
                .Include(p => p.Event)
                .ThenInclude(e => e.Prizes)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

            if (participation == null) return BadRequest("Сначала нужно подать заявку на мероприятие.");
            if (participation.Status == 1) return Ok("Вы уже подтвердили участие!");

            // Проверяем, совпадает ли секретный код из QR с кодом мероприятия
            if (participation.Event.VerificationCode != secretCode)
                return BadRequest("Неверный QR-код.");

            // Начисляем баллы
            participation.Status = 1; // Attended
            participation.CheckInDate = DateTime.UtcNow;

            var basePoints = participation.Event.Prizes.Where(p => !p.IsBonus).Sum(p => p.PointsValue);
            participation.FinalPoints = basePoints * participation.Event.ComplexityWeight;
            participation.User.TotalRating += participation.FinalPoints; // Обновляем рейтинг Топ-100

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = eventId });
        }

        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = Guid.Parse(_userManager.GetUserId(User));

            // Создаем само мероприятие
            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description,
                EventDate = model.EventDate,
                ComplexityWeight = model.ComplexityWeight,
                CategoryId = model.CategoryId,
                OrganizerId = userId,
                Status = 1, // Active
                VerificationCode = Guid.NewGuid().ToString().Substring(0, 8) // Секретный код для QR
            };

            _context.Events.Add(newEvent);

            // Создаем базовые баллы за участие
            if (model.BasePoints > 0)
            {
                _context.Prizes.Add(new Prize { EventId = newEvent.Id, Title = "Баллы за участие", PointsValue = model.BasePoints, IsBonus = false });
            }

            // Создаем материальный/карьерный бонус, если есть
            if (!string.IsNullOrEmpty(model.BonusTitle))
            {
                _context.Prizes.Add(new Prize { EventId = newEvent.Id, Title = model.BonusTitle, PointsValue = 0, IsBonus = true });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = newEvent.Id });
        }

        // ==========================================
        // 3. ЗАПИСЬ НА МЕРОПРИЯТИЕ (Только Участник)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Register(Guid eventId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));

            // Проверка: не записан ли уже?
            var exists = await _context.Participations.AnyAsync(p => p.EventId == eventId && p.UserId == userId);
            if (!exists)
            {
                var participation = new Participation
                {
                    EventId = eventId,
                    UserId = userId,
                    Status = 0 // 0 = Registered (Заявка подана)
                };
                _context.Participations.Add(participation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = eventId });
        }

        // ==========================================
        // 4. СИСТЕМА ПОДТВЕРЖДЕНИЯ И НАЧИСЛЕНИЕ БАЛЛОВ (Только Организатор)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> ConfirmParticipation(Guid participationId)
        {
            var participation = await _context.Participations
                .Include(p => p.Event)
                .ThenInclude(e => e.Prizes)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == participationId);

            if (participation == null || participation.Status == 1) return BadRequest("Уже подтверждено или не найдено.");

            // 1. Меняем статус на "Присутствовал"
            participation.Status = 1;
            participation.CheckInDate = DateTime.UtcNow;

            // 2. Считаем баллы (Сумма базовых баллов * Коэффициент сложности мероприятия)
            var basePoints = participation.Event.Prizes.Where(p => !p.IsBonus).Sum(p => p.PointsValue);
            participation.FinalPoints = basePoints * participation.Event.ComplexityWeight;

            // 3. Обновляем глобальный рейтинг участника (для Топ-100)
            participation.User.TotalRating += participation.FinalPoints;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Участие подтверждено, баллы начислены!", points = participation.FinalPoints });
        }
    }
}
