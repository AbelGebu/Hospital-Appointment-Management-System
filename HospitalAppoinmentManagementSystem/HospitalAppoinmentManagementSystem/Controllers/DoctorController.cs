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

        // View Appointments for logged-in Doctor
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

        // Approve Appointment
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
    }
}
