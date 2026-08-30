using System.ComponentModel.DataAnnotations;

namespace HospitalAppoinmentManagementSystem.Models
{
    public class Timeslot
    {
        [Key]
        public int TimeSlotId { get; set; }
        public int DoctorId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public virtual Doctor? Doctor { get; set; }
    }
}
