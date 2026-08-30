using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public virtual List<Appointment>? Appointments { get; set; }
    }
}
