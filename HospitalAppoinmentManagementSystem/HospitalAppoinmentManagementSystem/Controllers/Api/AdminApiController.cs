using HospitalAppoinmentManagementSystem.Data;
using HospitalAppoinmentManagementSystem.Dtos;
using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppoinmentManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly HospitalDbContext _db;
        public AdminApiController(HospitalDbContext db) => _db = db;

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _db.Doctors
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FullName,
                    Specialty = d.Specialty,
                    OpenSlots = d.TimeSlots
                        .Select(t => new TimeSlotDto { TimeSlotId = t.TimeSlotId, SlotTime = t.SlotTime })
                        .ToList()
                }).ToListAsync();
            return Ok(doctors);
        }

        [HttpPost("doctors")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDto dto)
        {
            var doctor = new Doctor
            {
                FullName = dto.FullName,
                Specialty = dto.Specialty,
                Email = dto.Email
            };
            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Doctor added", doctor.DoctorId });
        }

        [HttpPost("timeslots")]
        public async Task<IActionResult> AddTimeSlot([FromBody] AddTimeSlotDto dto)
        {
            var doctorExists = await _db.Doctors.AnyAsync(d => d.DoctorId == dto.DoctorId);
            if (!doctorExists) return NotFound(new { message = "Doctor not found" });

            _db.TimeSlots.Add(new Timeslot
            {
                DoctorId = dto.DoctorId,
                SlotTime = dto.SlotTime,
                IsBooked = false
            });
            await _db.SaveChangesAsync();
            return Ok(new { message = "Time slot added" });
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _db.Patients
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone
                }).ToListAsync();
            return Ok(patients);
        }
    }
}