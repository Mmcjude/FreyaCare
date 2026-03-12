using System.ComponentModel.DataAnnotations;

namespace FreyaCare.ViewModels;


public class DoctorAvailabilityViewModel
{
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Available Date")]
    public DateTime AvailableDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Start Time")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "End Time")]
    public TimeSpan EndTime { get; set; }
}