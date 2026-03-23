using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YouthParliamentApp.Data;
using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Models;

namespace YouthParliamentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Достаем все ивенты и подключаем категорию
            var events = await _context.Events
                .Include(e => e.Category)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            return View(events);
        }
    }
}

