using System.ComponentModel.DataAnnotations;

namespace FreyaCare.ViewModels;

public class ConsultationNoteViewModel
{
    public int AppointmentId { get; set; }

    [Display(Name = "Symptoms")]
    public string? Symptoms { get; set; }

    [Display(Name = "Diagnosis")]
    public string? Diagnosis { get; set; }

    [Display(Name = "Doctor Notes")]
    public string? DoctorNotes { get; set; }

    [Display(Name = "Prescription")]
    public string? Prescription { get; set; }
}