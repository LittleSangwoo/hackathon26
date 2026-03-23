using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YouthParliamentApp.Data;
using YouthParliamentApp.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. ПОДКЛЮЧАЕМ БАЗУ ДАННЫХ (именно этого куска кода тебе не хватало)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. ПОДКЛЮЧАЕМ IDENTITY (Систему ролей и пользователей)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Отключаем сложные пароли для удобства на хакатоне
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders().AddDefaultUI(); 

// Добавляем поддержку MVC (контроллеры и вьюшки)
builder.Services.AddControllersWithViews();

// Настройка политик (Policy-based authorization) — более гибкий подход
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrganizerOnly", policy => policy.RequireRole("Organizer"));
    options.AddPolicy("ObserverOnly", policy => policy.RequireRole("Observer"));
    options.AddPolicy("ParticipantOnly", policy => policy.RequireRole("Participant"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Указываем, куда отправлять юзера, если он не залогинен
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // ПЕРВЫМ: Кто ты? (Проверка куки/паспорта)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
