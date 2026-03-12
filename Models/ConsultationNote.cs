namespace FreyaCare.Models;

public class ConsultationNote
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? DoctorNotes { get; set; }
    public string? Prescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Appointment? Appointment { get; set; }
}