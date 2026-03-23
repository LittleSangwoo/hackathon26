using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Регистрация Identity с поддержкой ролей
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false; // Упростим для хакатона
})
.AddRoles<IdentityRole>() // Важно: добавляем поддержку ролей!
.AddEntityFrameworkStores<ApplicationDbContext>();

// Настройка политик (Policy-based authorization) — более гибкий подход
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrganizerOnly", policy => policy.RequireRole("Organizer"));
    options.AddPolicy("ObserverOnly", policy => policy.RequireRole("Observer"));
    options.AddPolicy("ParticipantOnly", policy => policy.RequireRole("Participant"));
});

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
