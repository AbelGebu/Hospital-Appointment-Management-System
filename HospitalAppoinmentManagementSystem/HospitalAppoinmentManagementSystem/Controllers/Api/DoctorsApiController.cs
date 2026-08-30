using HospitalAppoinmentManagementSystem.Data;
using HospitalAppoinmentManagementSystem.Dtos;
using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/doctors")]
public class DoctorsApiController : ControllerBase
{
    private readonly HospitalDbContext _db;
    public DoctorsApiController(HospitalDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doctors = await _db.Doctors
            .Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialty = d.Specialty,
                OpenSlots = d.TimeSlots
                    .Where(t => !t.IsBooked && t.SlotTime > DateTime.Now)
                    .Select(t => new TimeSlotDto { TimeSlotId = t.TimeSlotId, SlotTime = t.SlotTime })
                    .ToList()
            }).ToListAsync();

        return Ok(doctors);
    }
}