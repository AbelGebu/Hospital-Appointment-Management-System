using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppoinmentManagementSystem.Data;

namespace HospitalAppoinmentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HospitalDbContext _db;

        public AdminController(HospitalDbContext db) => _db = db;

        // View Doctor list
        public IActionResult Doctors()
        {
            var doctors = _db.Doctors.Include(d => d.TimeSlots).ToList();
            return View(doctors);
        }

        // Add Doctor (GET)
        public IActionResult AddDoctor() => View();

        // Add Doctor (POST)
        [HttpPost]
        public IActionResult AddDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _db.Doctors.Add(doctor);
                _db.SaveChanges();
                return RedirectToAction("Doctors");
            }
            return View(doctor);
        }

        // View Patient list
        public IActionResult Patients()
        {
            var patients = _db.Patients.ToList();
            return View(patients);
        }

        // Add Time Slot for a Doctor
        [HttpPost]
        public IActionResult AddTimeSlot(int doctorId, DateTime slotTime)
        {
            _db.TimeSlots.Add(new Timeslot
            {
                DoctorId = doctorId,
                SlotTime = slotTime,
                IsBooked = false
            });
            _db.SaveChanges();
            return RedirectToAction("Doctors");
        }
    }
}
