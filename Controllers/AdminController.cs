using FreyaCare.Data;
using FreyaCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.PatientCount = await _context.Users
            .CountAsync(u => u.Role == "Patient");

        ViewBag.DoctorCount = await _context.Users
            .CountAsync(u => u.Role == "Doctor");

        ViewBag.AppointmentCount = await _context.Appointments
            .CountAsync();

        ViewBag.TodayAppointments = await _context.Appointments
            .CountAsync(a => a.AppointmentDate.Date == DateTime.Today);

        ViewBag.PendingAppointments = await _context.Appointments
            .CountAsync(a => a.Status == "Pending");

        ViewBag.CompletedAppointments = await _context.Appointments
            .CountAsync(a => a.Status == "Completed");

        var recentAppointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .Take(5)
            .ToListAsync();

        return View(recentAppointments);
    }
    

    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .OrderBy(u => u.Role)
            .ThenBy(u => u.FullName)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(User model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users.FindAsync(model.Id);

        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.PersonalCode = model.PersonalCode;
        user.PhoneNumber = model.PhoneNumber;
        user.Email = model.Email;
        user.Role = model.Role;

        await _context.SaveChangesAsync();

        return RedirectToAction("Users");
    }


    [HttpGet]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        var hasAppointment = await _context.Appointments
            .AnyAsync(a => a.PatientId == id || a.DoctorId == id);

        if (hasAppointment)
        {
            TempData["ErrorMessage"] = $"{user.FullName} has an appointment and cannot be deleted.";
            return RedirectToAction("Users");
        }

        return View(user);
    }

    [HttpPost, ActionName("DeleteUser")]
    public async Task<IActionResult> DeleteUserConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        var hasAppointment = await _context.Appointments
            .AnyAsync(a => a.PatientId == id || a.DoctorId == id);

        if (hasAppointment)
        {
            TempData["ErrorMessage"] = $"{user.FullName} has an appointment and cannot be deleted.";
            return RedirectToAction("Users");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"{user.FullName} was deleted successfully.";
        return RedirectToAction("Users");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }

    [HttpPost, ActionName("DeleteAppointment")]
    public async Task<IActionResult> DeleteAppointmentConfirmed(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Appointments");
    }
}