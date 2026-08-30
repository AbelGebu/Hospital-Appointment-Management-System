namespace HospitalAppoinmentManagementSystem.Dtos
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = "";
        public string Specialty { get; set; } = "";
        public List<TimeSlotDto> OpenSlots { get; set; } = new();
    }

    public class TimeSlotDto
    {
        public int TimeSlotId { get; set; }
        public DateTime SlotTime { get; set; }
    }

    public class BookAppointmentDto
    {
        public int TimeSlotId { get; set; }
    }

    public class AddDoctorDto
    {
        public string FullName { get; set; } = "";
        public string Specialty { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class AddTimeSlotDto
    {
        public int DoctorId { get; set; }
        public DateTime SlotTime { get; set; }
    }

    public class PatientDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public string Status { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public string PatientName { get; set; } = "";
        public DateTime SlotTime { get; set; }
    }
}