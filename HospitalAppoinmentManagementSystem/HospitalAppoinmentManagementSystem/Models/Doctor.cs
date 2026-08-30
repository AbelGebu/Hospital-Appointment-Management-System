using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Specialty { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public virtual List<Timeslot>? TimeSlots { get; set; }
    }
}
