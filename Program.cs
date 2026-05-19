using FreyaCare.Data;
using FreyaCare.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Applying migrations first so Azure and local DB has the tables
        context.Database.Migrate();

        // Admin user
        if (!context.Users.Any(u => u.PersonalCode == "ADM001"))
        {
            context.Users.Add(new User
            {
                FullName = "System Admin",
                PersonalCode = "ADM001",
                PhoneNumber = "28888888",
                Email = "admin@freyacare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin"
            });
        }

        // Doctors stored in Users table
        var doctors = new List<User>
        {
            new User
            {
                FullName = "Dr. Anna Ozola",
                PersonalCode = "DOC001",
                PhoneNumber = "29999999",
                Email = "anna.ozola@freyacare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
                Role = "Doctor"
            },
            new User
            {
                FullName = "Dr. Janis Berzins",
                PersonalCode = "DOC002",
                PhoneNumber = "29999998",
                Email = "janis.berzins@freyacare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
                Role = "Doctor"
            },
            new User
            {
                FullName = "Dr. Elina Kalnina",
                PersonalCode = "DOC003",
                PhoneNumber = "29999997",
                Email = "elina.kalnina@freyacare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
                Role = "Doctor"
            }
        };

        foreach (var doctor in doctors)
        {
            if (!context.Users.Any(u => u.PersonalCode == doctor.PersonalCode))
            {
                context.Users.Add(doctor);
            }
        }

        context.SaveChanges();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration/seeding failed during startup.");
    }
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();