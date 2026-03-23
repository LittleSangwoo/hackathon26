using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Data;
using YouthParliamentApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouthParliamentApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Получаем ID текущего пользователя
            var rawUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(rawUserId))
            {
                // Если юзер не залогинен, кидаем его на страницу входа
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(rawUserId);

            // 2. Загружаем пользователя со всеми связями
            var user = await _context.Users  // Чтобы посчитать участие (для Участника)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            return View(user);
        }
    }
}
