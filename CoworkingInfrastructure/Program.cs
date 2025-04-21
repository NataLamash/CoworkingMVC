using CoworkingInfrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoworkingDomain.Model;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ─── 1. Реєструємо DbContext з логуванням SQL ───────────────────────────────
builder.Services.AddDbContext<CoworkingDbContext>(opts =>
    opts
      .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
      // Виводити кожен SQL у консоль
      .LogTo(Console.WriteLine, LogLevel.Information)
      .EnableSensitiveDataLogging()
);

// ─── 2. Налаштовуємо Identity і його опції ─────────────────────────────────
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" +
        "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя" +
        "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
    // за бажанням можна додати ще options.Password, Lockout тощо
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CoworkingDbContext>()
    .AddDefaultTokenProviders();

// ─── 3. Інші сервіси ───────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ─── 4. Middleware ───────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ─── 5. Ініціалізація ролей/адмінів ───────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        // TODO: залогувати ex
    }
}

// ─── 6. Маршрутизація ─────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
