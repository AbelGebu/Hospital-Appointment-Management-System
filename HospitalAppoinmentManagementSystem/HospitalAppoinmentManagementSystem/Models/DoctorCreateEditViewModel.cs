using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class DoctorCreateEditViewModel
    {
        public int DoctorId { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public string Specialization { get; set; }
    }
}
