using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppoinmentManagementSystem.Data;

namespace HospitalAppoinmentManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly HospitalDbContext _db;

        public DoctorController(HospitalDbContext db) => _db = db;

        
        public IActionResult MyAppointments()
        {
            var doctorEmail = User.Identity?.Name;
            var appointments = _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Timeslot)
                .Where(a => a.Timeslot.Doctor.Email == doctorEmail)
                .ToList();

            return View(appointments);
        }

        
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var appointment = _db.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = "Approved";
                _db.SaveChanges();
            }
            return RedirectToAction("MyAppointments");
        }
        [HttpGet]
        public IActionResult Prescribe(int appointmentId)
        {
            var appointment = _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment == null) return NotFound();

            ViewBag.PatientName = appointment.Patient?.FullName;
            return View(new Prescription { AppointmentId = appointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Prescribe(Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _db.Prescriptions.Add(prescription);
                _db.SaveChanges();
                return RedirectToAction("MyAppointments");
            }
            return View(prescription);
        }
    }
}
