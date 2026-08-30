using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        public int AppointmentId { get; set; }

        [Required]
        public string MedicationDetails { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime DatePrescribed { get; set; } = DateTime.Now;

        public virtual Appointment? Appointment { get; set; }
    }
}