using FreyaCare.Data;
using FreyaCare.Models;
using FreyaCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Doctor")]
public class ConsultationController : Controller
{
    private readonly ApplicationDbContext _context;

    public ConsultationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound();

        var model = new ConsultationNoteViewModel
        {
            AppointmentId = appointmentId
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConsultationNoteViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var note = new ConsultationNote
        {
            AppointmentId = model.AppointmentId,
            Symptoms = model.Symptoms,
            Diagnosis = model.Diagnosis,
            DoctorNotes = model.DoctorNotes,
            Prescription = model.Prescription
        };

        _context.ConsultationNotes.Add(note);
        await _context.SaveChangesAsync();

        return RedirectToAction("Dashboard", "Doctor");
    }
}