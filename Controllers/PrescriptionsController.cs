using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using RemindONServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace RemindONServer.Controllers
{
    [Route("api/devices/{serialNumber}/prescriptions")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize("ShouldBeAnUser")]
    [ApiController]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/devices/{serialNumber}/prescriptions?date=2021-09-10
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PrescriptionViewModel>>> GetPrescriptions([FromRoute] string serialNumber, [FromQuery] string date)
        {
            if (Enum.TryParse<DayOfWeek>(date, out var dayOfWeek))
            {
                return Ok(_context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber && p.WeekDays.Contains(dayOfWeek)).Select(p => new PrescriptionViewModel
                {
                    text1 = p.text1,
                    text2 = p.text2,
                    WeekDays = p.WeekDays,
                    DayTimes = p.DayTimes
                }).AsEnumerable());
            }

            return Ok(_context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber).Select(p => new PrescriptionViewModel
            {
                text1 = p.text1,
                text2 = p.text2,
                WeekDays = p.WeekDays,
                DayTimes = p.DayTimes
            }).AsEnumerable());
        }

        // GET: api/devices/{serialNumber}/prescriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionViewModel>> GetPrescription([FromRoute] string serialNumber, [FromRoute] int id)
        {
            var prescription = _context.Prescriptions.FirstOrDefault(p => p.DeviceSerialNumber == serialNumber && p.ID == id);
            if (prescription == null)
                return NotFound();

            return Ok(new PrescriptionViewModel
            {
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes
            });
        }

        // PUT: api/devices/{serialNumber}/prescriptions/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PrescriptionViewModel>> GetPrescription([FromRoute] string serialNumber, [FromRoute] int id, [FromBody] PrescriptionViewModel prescriptionViewModel)
        {
            var prescription = _context.Prescriptions.FirstOrDefault(p => p.DeviceSerialNumber == serialNumber && p.ID == id);
            if (prescription == null)
                return NotFound();

            prescription.text1 = prescription.text1;
            prescription.text2 = prescription.text2;
            prescription.WeekDays = prescription.WeekDays;
            prescription.DayTimes = prescription.DayTimes;
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK);
        }

        // DELETE: api/devices/{serialNumber}/prescriptions/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<PrescriptionViewModel>> DeletePrescription([FromRoute] string serialNumber, [FromRoute] int id)
        {
            var prescription = _context.Prescriptions.FirstOrDefault(p => p.DeviceSerialNumber == serialNumber && p.ID == id);
            if (prescription == null)
                return NotFound();
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK);
        }

        // GET: api/devices/{serialNumber}/prescriptions/{id}/checks
        [HttpGet("{id}/checks")]
        public async Task<ActionResult<IEnumerable<CheckViewModel>>> GetChecksForPrescription([FromRoute] string serialNumber, [FromRoute] int id)
        {
            var prescription = _context.Prescriptions.FirstOrDefault(p => p.DeviceSerialNumber == serialNumber && p.ID == id);
            if (prescription == null)
                return NotFound();

            return Ok(_context.Checks.Where(c => c.PrescriptionID == prescription.ID).Select(c => new CheckViewModel
            {
                ID = c.ID,
                PrescriptionID = c.PrescriptionID,
                Flag = c.Flag
            }).AsEnumerable());
        }        

        //POST: api/devices/{serialNumber}/prescriptions
        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromRoute] string serialNumber, [FromBody] PrescriptionViewModel prescription)
        {
            var remindONDevice = await _context.RemindONDevices.FindAsync(serialNumber);

            if (remindONDevice == null)
            {
                return NotFound();
            }

            await _context.Prescriptions.AddAsync(new Prescription
            {
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes
            });
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
