using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int TimeSlotId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Cancelled
        public virtual Patient? Patient { get; set; }
        public virtual Timeslot? Timeslot { get; set; }
    }
}
