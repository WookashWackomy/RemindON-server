using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemindONServer.Auth;
using RemindONServer.Domain.Models;
using RemindONServer.Domain.Persistence.Contexts;
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
            return EmbeddedTimeStampHelper.GetFormattedDateReduced(DateTime.Now);
        }

        // GET api/embedded/prescriptions
        [HttpGet("prescriptions")]
        public async Task<ActionResult<IEnumerable<EmbeddedPrescriptionViewModel>>> GetPrescriptionsForDevice()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var credentials = authHeader.Split(new[] { ':' }, 2);
            var serialNumber = credentials[0];

            var prescriptions = _context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber)
                .AsEnumerable() // TODO evaluate at the end of query
                .Select(p => new EmbeddedPrescriptionViewModel
                {
                    ID = p.ID,
                    text1 = p.text1,
                    text2 = p.text2,
                    WeekDays = p.WeekDays,
                    DayTimes = p.DayTimes.Select(dt => ((int)dt.TotalSeconds).ToString())
                });

            return Ok($"[{string.Join(';',prescriptions)}]");
        }

        // POST api/embedded/checks
        [HttpPost("checks")]
        public async Task<IActionResult> PostPrescriptionCheck( [FromBody] CheckViewModel checkViewModel)
        {

            var prescription = _context.Prescriptions.FirstOrDefault(p => p.ID == checkViewModel.PrescriptionID);
            if (prescription == null)
            {
                return BadRequest("Prescrption of given id not found");
            }

            var newCheck = new Check
            {
                Flag = Convert.ToBoolean(checkViewModel.Flag),
                TimeStamp = DateTime.Parse(checkViewModel.TimeStamp),
                PrescriptionID = checkViewModel.PrescriptionID
            };

            _context.Checks.Add(newCheck);
            await _context.SaveChangesAsync();

            return new ObjectResult(newCheck) { StatusCode = StatusCodes.Status201Created };
        }

        //<summary>Get checks for prescription </summary> 
        //<param name="prescriptionId"> prescription ID </param>
        //<returns> String of an array of checks: ["{ID},{PrescriptionID},{Flag},{TimeStamp}",...,"{ID},{PrescriptionID},{Flag},{TimeStamp}"] <returns/>
        // GET api/embedded/checks?prescriptionId=124
        [HttpGet("checks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmbeddedCheckViewModel>>> GetChecks([FromQuery]int prescriptionId)
        {

            var prescription = _context.Prescriptions.FirstOrDefault(p => p.ID == prescriptionId);
            if (prescription == null)
            {
                return BadRequest("Prescrption of given id not found");
            }

            var checks = _context.Checks.Where(p => p.PrescriptionID == prescriptionId)
                        .Select(p => new EmbeddedCheckViewModel
                        {   ID = p.ID,
                            Flag = Convert.ToInt32(p.Flag),
                            PrescriptionID = p.PrescriptionID,
                            TimeStamp = EmbeddedTimeStampHelper.GetFormattedDateReduced(p.TimeStamp)
                        }).AsEnumerable();

            return Ok($"[{string.Join(';',checks)}]");
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

            //temp code
            var prescription = new Prescription
            {
                DeviceSerialNumber = deviceViewModel.SerialNumber,
                text1 = "print pies",
                text2 = "print php",
                WeekDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                DayTimes = new List<TimeSpan> { new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0) }
            };

            _context.Prescriptions.Add(prescription);
            //end of temp code

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
