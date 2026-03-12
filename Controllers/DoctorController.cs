using FreyaCare.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAppointment(int id)
    {
        var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == doctorId);

        if (appointment == null)
            return NotFound();

        appointment.Status = "Approved";
        await _context.SaveChangesAsync();

        return RedirectToAction("Appointments");
    }

    public async Task<IActionResult> Appointments()
    {
        var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.ConsultationNotes)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return View(appointments);
    }
}
