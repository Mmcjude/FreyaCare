using System.Security.Claims;
using FreyaCare.Data;
using FreyaCare.Models;
using FreyaCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Patient")]
public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public AppointmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Book()
    {
        var doctors = await _context.Users
            .Where(u => u.Role == "Doctor")
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            })
            .ToListAsync();

        var model = new BookAppointmentViewModel
        {
            Doctors = doctors
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Book(BookAppointmentViewModel model)
    {
        model.Doctors = await _context.Users
            .Where(u => u.Role == "Doctor")
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            })
            .ToListAsync();

        if (!ModelState.IsValid)
            return View(model);

        var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = model.DoctorId,
            AppointmentDate = model.AppointmentDate,
            Status = "Pending",
            ConsultationType = "Online"
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        appointment.MeetingLink = $"https://meet.jit.si/freyacare-appointment-{appointment.Id}";
        await _context.SaveChangesAsync();

        return RedirectToAction("MyAppointments");
    }

    public async Task<IActionResult> MyAppointments()
    {
        var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return View(appointments);
    }

public async Task<IActionResult> ViewResult(int id)
    {
        var patientId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.ConsultationNotes)
            .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }
}