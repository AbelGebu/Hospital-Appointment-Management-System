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

        
        [AllowAnonymous]
        public IActionResult Doctors()
        {
            var doctors = _db.Doctors.Include(d => d.TimeSlots).ToList();
            return View(doctors);
        }

        
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

        
        [HttpPost]
        public IActionResult Book(int timeSlotId)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home");
            }

            
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

        [HttpGet]
        public IActionResult Profile()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

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
                _db.SaveChanges();
            }

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Patient patientInput)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

            var patient = _db.Patients.FirstOrDefault(p => p.Email == email);
            if (patient != null)
            {
                patient.FullName = patientInput.FullName;
                patient.Phone = patientInput.Phone;
                _db.SaveChanges();
                ViewBag.Message = "Profile updated successfully!";
            }

            return View(patient ?? patientInput);
        }
        [HttpGet]
        public IActionResult MyPrescriptions()
        {
            var email = User.Identity?.Name;
            var patient = _db.Patients.FirstOrDefault(p => p.Email == email);
            if (patient == null) return RedirectToAction("Index", "Home");

            var prescriptions = _db.Prescriptions
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Timeslot)
                        .ThenInclude(t => t.Doctor)
                .Where(p => p.Appointment.PatientId == patient.PatientId)
                .ToList();

            return View(prescriptions);
        }
    }
}
