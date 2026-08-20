using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppoinmentManagementSystem.Data;

namespace HospitalAppoinmentManagementSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly HospitalDbContext _db;

        public PatientController(HospitalDbContext db) => _db = db;

        // View Available Doctors and Time Slots
        [AllowAnonymous]
        public IActionResult Doctors()
        {
            var doctors = _db.Doctors.Include(d => d.TimeSlots).ToList();
            return View(doctors);
        }

        // View Patient Profile / Appointments
        public IActionResult MyAppointments()
        {
            var email = User.Identity?.Name;

            // Ensure Patient record exists in domain DB
            var patient = _db.Patients.FirstOrDefault(p => p.Email == email);
            if (patient == null)
            {
                patient = new Patient { FullName = User.Identity?.Name ?? "Patient", Email = email! };
                _db.Patients.Add(patient);
                _db.SaveChanges();
            }

            var appointments = _db.Appointments
                .Include(a => a.Timeslot)
                    .ThenInclude(t => t.Doctor)
                .Where(a => a.PatientId == patient.PatientId)
                .ToList();

            return View(appointments);
        }

        // Schedule / Book Appointment
        [HttpPost]
        public IActionResult Book(int timeSlotId)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home");
            }

            // 1. Ensure Patient record exists before saving appointment
            var patient = _db.Patients.FirstOrDefault(p => p.Email == email);
            if (patient == null)
            {
                patient = new Patient
                {
                    FullName = email.Split('@')[0],
                    Email = email,
                    Phone = "N/A"
                };
                _db.Patients.Add(patient);
                _db.SaveChanges(); // Persists Patient to generate valid PatientId
            }

            // 2. Find timeslot and save appointment
            var slot = _db.TimeSlots.FirstOrDefault(t => t.TimeSlotId == timeSlotId);
            if (slot != null && !slot.IsBooked)
            {
                slot.IsBooked = true;

                var appointment = new Appointment
                {
                    PatientId = patient.PatientId,
                    TimeSlotId = timeSlotId,
                    Status = "Pending"
                };

                _db.Appointments.Add(appointment);
                _db.SaveChanges();
            }

            return RedirectToAction("MyAppointments");
        }

        // Cancel Appointment
        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var appointment = _db.Appointments.Include(a => a.Timeslot).FirstOrDefault(a => a.AppointmentId == id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                if (appointment.Timeslot != null)
                {
                    appointment.Timeslot.IsBooked = false;
                }
                _db.SaveChanges();
            }
            return RedirectToAction("MyAppointments");
        }
    }
}
