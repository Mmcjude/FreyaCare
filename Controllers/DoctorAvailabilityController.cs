using System.Security.Claims;
using FreyaCare.Data;
using FreyaCare.Models;
using FreyaCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorAvailabilityController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorAvailabilityController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(DoctorAvailabilityViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.EndTime <= model.StartTime)
        {
            ModelState.AddModelError("", "End time must be later than start time.");
            return View(model);
        }

        var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var availability = new DoctorAvailability
        {
            DoctorId = doctorId,
            AvailableDate = model.AvailableDate.Date,
            StartTime = model.StartTime,
            EndTime = model.EndTime
        };

        _context.DoctorAvailabilities.Add(availability);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyAvailability");
    }

    public async Task<IActionResult> MyAvailability()
    {
        var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var slots = await _context.DoctorAvailabilities
            .Where(x => x.DoctorId == doctorId)
            .OrderBy(x => x.AvailableDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        return View(slots);
    }
}