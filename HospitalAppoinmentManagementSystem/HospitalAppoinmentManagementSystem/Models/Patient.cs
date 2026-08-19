using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public virtual List<Appointment> Appointments { get; set; }
    }
}
