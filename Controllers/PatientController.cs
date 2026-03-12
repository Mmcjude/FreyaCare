using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreyaCare.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
