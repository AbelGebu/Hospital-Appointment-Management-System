using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class AppointmentCreateEditViewModel
    {
        public int AppointmentId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

        [Required]
        public int PatientId { get; set; }
        public string? PatientName { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
