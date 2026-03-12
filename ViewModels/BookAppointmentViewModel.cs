using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreyaCare.ViewModels;

public class BookAppointmentViewModel
{
    [Required]
    [Display(Name = "Doctor")]
    public int DoctorId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Appointment Date")]
    public DateTime AppointmentDate { get; set; }

    public List<SelectListItem> Doctors { get; set; } = new();
}