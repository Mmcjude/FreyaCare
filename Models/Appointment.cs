namespace FreyaCare.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string ConsultationType { get; set; } = "Online";
    public string? MeetingLink { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Patient { get; set; }
    public User? Doctor { get; set; }

    public ICollection<ConsultationNote> ConsultationNotes { get; set; } = new List<ConsultationNote>();
}