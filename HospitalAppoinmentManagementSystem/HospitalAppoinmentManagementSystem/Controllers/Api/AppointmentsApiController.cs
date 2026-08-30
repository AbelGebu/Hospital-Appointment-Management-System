using HospitalAppoinmentManagementSystem.Data;
using HospitalAppoinmentManagementSystem.Dtos;
using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalAppoinmentManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly HospitalDbContext _db;
        public AppointmentsApiController(HospitalDbContext db) => _db = db;

        [HttpGet("mine")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var appointments = await _db.Appointments
                .Include(a => a.Timeslot).ThenInclude(t => t.Doctor)
                .Where(a => a.Patient.Email == email)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    Status = a.Status,
                    DoctorName = a.Timeslot.Doctor.FullName,
                    SlotTime = a.Timeslot.SlotTime
                }).ToListAsync();
            return Ok(appointments);
        }

        [HttpPost("book")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Book([FromBody] BookAppointmentDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Email == email);
            if (patient is null)
            {
                patient = new Patient { FullName = email!.Split('@')[0], Email = email!, Phone = "N/A" };
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();
            }

            var slot = await _db.TimeSlots.FindAsync(dto.TimeSlotId);
            if (slot is null || slot.IsBooked) return BadRequest(new { message = "Slot not available" });

            slot.IsBooked = true;
            _db.Appointments.Add(new Appointment
            {
                PatientId = patient.PatientId,
                TimeSlotId = slot.TimeSlotId,
                Status = "Pending"
            });
            await _db.SaveChangesAsync();
            return Ok(new { message = "Booked successfully" });
        }

        [HttpPost("cancel/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Cancel(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var appointment = await _db.Appointments
                .Include(a => a.Timeslot)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment is null) return NotFound();
            if (appointment.Patient.Email != email) return Forbid();

            appointment.Status = "Cancelled";
            if (appointment.Timeslot != null) appointment.Timeslot.IsBooked = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Cancelled successfully" });
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Approve(int id)
        {
            var doctorEmail = User.FindFirstValue(ClaimTypes.Email);
            var appointment = await _db.Appointments
                .Include(a => a.Timeslot).ThenInclude(t => t.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment is null) return NotFound();
            if (appointment.Timeslot.Doctor.Email != doctorEmail) return Forbid();

            appointment.Status = "Approved";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Approved successfully" });
        }

        [HttpGet("doctor-schedule")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorSchedule()
        {
            var doctorEmail = User.FindFirstValue(ClaimTypes.Email);
            var appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Timeslot).ThenInclude(t => t.Doctor)
                .Where(a => a.Timeslot.Doctor.Email == doctorEmail)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    Status = a.Status,
                    PatientName = a.Patient.FullName,
                    SlotTime = a.Timeslot.SlotTime
                }).ToListAsync();
            return Ok(appointments);
        }
    }
}