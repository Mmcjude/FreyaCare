namespace FreyaCare.Models;

public class DoctorAvailability
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DateTime AvailableDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public User? Doctor { get; set; }
}