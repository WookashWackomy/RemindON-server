using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemindONServer.Auth;
using RemindONServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemindONServer.Controllers
{
    [Route("api/embedded")]
    [BasicAuthorization]
    [ApiController]
    public class EmbeddedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmbeddedController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        // GET api/embedded/time
        [HttpGet("time")]
        public ActionResult<string> GetTime()
        {
            var currentDate = DateTime.Now;
            return $"{currentDate:yyyy-MM-dd HH:mm:ss} {(int)currentDate.DayOfWeek}";
        }

        // GET api/embedded/prescriptions
        [HttpGet("prescriptions")]
        public async Task<ActionResult<IEnumerable<PrescriptionViewModel>>> GetPrescriptionsForDevice([FromRoute] string serialNumber)
        {
            var device = _context.RemindONDevices.FirstOrDefault(d => d.SerialNumber == serialNumber);
            if (device == null)
            {
                return BadRequest("Device of given serial number not found");
            }
            var prescriptions = _context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber)
                .Select(p => new PrescriptionViewModel
                {
                    text1 = p.text1,
                    text2 = p.text2,
                    WeekDays = p.WeekDays,
                    DayTimes = p.DayTimes
                }).AsEnumerable();

            return Ok(prescriptions);
        }

        // POST api/embedded/devices/{serialNumber}/prescriptions/{id}/checks
        [HttpPost("prescriptions/{id}/checks")]
        public async Task<IActionResult> PostPrescriptionCheck([FromRoute] string serialNumber, [FromRoute] int id, [FromBody] CheckViewModel checkViewModel)
        {
            var device = _context.RemindONDevices.FirstOrDefault(d => d.SerialNumber == serialNumber);
            if (device == null)
            {
                return BadRequest("Device of given serial number not found");
            }

            var prescription = _context.Prescriptions.FirstOrDefault(p => p.ID == id);
            if (prescription == null)
            {
                return BadRequest("Prescrption of given id not found");
            }

            var newCheck = new Check
            {
                Flag = checkViewModel.Flag,
                TimeStamp = checkViewModel.TimeStamp,
                PrescriptionID = id
            };

            _context.Checks.Add(newCheck);
            await _context.SaveChangesAsync();

            return new ObjectResult(newCheck) { StatusCode = StatusCodes.Status201Created };
        }

        // POST: api/embedded/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RemindONDevice>> RegisterDevice([FromBody] RemindONDeviceRegisterViewModel deviceViewModel)
        {
            if (_context.RemindONDevices.FirstOrDefault(d => d.SerialNumber == deviceViewModel.SerialNumber) != null)
                return StatusCode(StatusCodes.Status409Conflict);
            var password = RandomString(10);
            var model = new RemindONDevice
            {
                SerialNumber = deviceViewModel.SerialNumber,
                Password = password
            };

            _context.RemindONDevices.Add(model);
            await _context.SaveChangesAsync();

            //temp code
            var prescription = new Prescription
            {
                DeviceSerialNumber = deviceViewModel.SerialNumber,
                text1 = "print pies",
                text2 = "print php",
                WeekDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                DayTimes = new List<TimeSpan> { new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0) }
            };

            _context.RemindONDevices.Add(model);
            await _context.SaveChangesAsync();



            return new ObjectResult(model.Password) { StatusCode = StatusCodes.Status201Created };
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
