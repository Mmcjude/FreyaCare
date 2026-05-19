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
            Doctors = doctors,
            AppointmentDate = DateTime.Now
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

        if (model.AppointmentDate <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(model.AppointmentDate), "You cannot book an appointment in the past.");
        }

        var doctorExists = await _context.Users
            .AnyAsync(u => u.Id == model.DoctorId && u.Role == "Doctor");

        if (!doctorExists)
        {
            ModelState.AddModelError(nameof(model.DoctorId), "Selected doctor does not exist.");
        }

        var alreadyBooked = await _context.Appointments.AnyAsync(a =>
            a.DoctorId == model.DoctorId &&
            a.AppointmentDate == model.AppointmentDate &&
            a.Status != "Cancelled");

        if (alreadyBooked)
        {
            ModelState.AddModelError(nameof(model.AppointmentDate), "This appointment time is already booked.");
        }

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

        var oldAppointments = await _context.Appointments
            .Where(a => a.PatientId == patientId
                     && a.AppointmentDate < DateTime.Now
                     && a.Status == "Pending")
            .ToListAsync();

        foreach (var appointment in oldAppointments)
        {
            appointment.Status = "Completed";
        }

        await _context.SaveChangesAsync();

        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return View(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);

        if (appointment == null)
            return NotFound();

        if (appointment.Status == "Completed")
        {
            TempData["Error"] = "Completed appointments cannot be cancelled.";
            return RedirectToAction("MyAppointments");
        }

        appointment.Status = "Cancelled";

        await _context.SaveChangesAsync();

        return RedirectToAction("MyAppointments");
    }

    public async Task<IActionResult> ViewResult(int id)
    {
        var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.ConsultationNotes)
            .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }
}