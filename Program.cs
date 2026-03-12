using FreyaCare.Data;
using FreyaCare.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    // Apply migrations first so Azure/local DB has the tables
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