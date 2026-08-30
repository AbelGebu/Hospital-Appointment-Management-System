using HospitalAppoinmentManagementSystem.Data;
using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppoinmentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HospitalDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(HospitalDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Doctors()
        {
            var doctors = _db.Doctors.Include(d => d.TimeSlots).ToList();
            return View(doctors);
        }

        public IActionResult AddDoctor() => View();

        [HttpPost]
        public async Task<IActionResult> AddDoctor(Doctor doctor, string password = "Doctor123!")
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByEmailAsync(doctor.Email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = doctor.Email,
                        Email = doctor.Email,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Doctor");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(doctor);
                    }
                }

                
                _db.Doctors.Add(doctor);
                await _db.SaveChangesAsync();
                return RedirectToAction("Doctors");
            }
            return View(doctor);
        }

        public IActionResult Patients()
        {
            var patients = _db.Patients.ToList();
            return View(patients);
        }

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