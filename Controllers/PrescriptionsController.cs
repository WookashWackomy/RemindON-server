using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RemindONServer.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RemindONServer.Domain.Services;
using RemindONServer.Domain.Persistence.Contexts;
using RemindONServer.Domain.Models.ModelMappers;
using RemindONServer.Domain.Services.Communication;

namespace RemindONServer.Controllers
{
    [Produces("application/json")]
    [Route("api/devices/{serialNumber}/prescriptions")] //TODO switch to api/prescriptions with query params
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize("ShouldBeAnUser")]
    [ApiController]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context; //TODO encapsulate
        private readonly IPrescriptionsService _prescriptionsService;

        public PrescriptionsController(ApplicationDbContext context, IPrescriptionsService prescriptionsService)
        {
            _context = context;
            _prescriptionsService = prescriptionsService ?? throw new ArgumentNullException(nameof(prescriptionsService));
        }

        // GET: api/devices/{serialNumber}/prescriptions?date=2021-09-10
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PrescriptionViewModel>>> GetPrescriptions([FromRoute] string serialNumber, [FromQuery] string date)
        {
            if (Enum.TryParse<DayOfWeek>(date, out var dayOfWeek))
            {
                return Ok(_context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber && p.WeekDays.Contains(dayOfWeek))
                .AsEnumerable()
                .Select(p => new PrescriptionViewModel
                {
                    text1 = p.text1,
                    text2 = p.text2,
                    WeekDays = p.WeekDays,
                    DayTimes = p.DayTimes.Select(ts => ts.ToString())
                }));
            }

            return Ok(_context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber)
                .AsEnumerable()
                .Select(p => new PrescriptionViewModel
                {
                    ID = p.ID,
                    text1 = p.text1,
                    text2 = p.text2,
                    WeekDays = p.WeekDays,
                    DayTimes = p.DayTimes.Select(ts => ts.ToString())
                }));
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
                ID = prescription.ID,
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes.Select(ts => ts.ToString())
            });
        }
        /// <summary>
        /// changes the data of a prescription
        /// </summary>
        /// <remarks>
        ///  PUT: api/devices/125246234436424/prescriptions/{id}
        ///  {
        ///		"text1": "print pies2",
        ///		"text2": "print php2",
        ///		"weekDays": [
        ///			2,
        ///			3,
        ///			6
        ///		],
        ///		"dayTimes": [
        ///			"09:00:00",
        ///			"11:00:00",
        ///			"13:00:00"
        ///		]
        ///   }
        /// </remarks>
        /// <returns> status code with optional error message</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrescriptionViewModel>> ChangePrescription([FromRoute] string serialNumber, [FromRoute] int id, [FromBody] PrescriptionViewModel prescriptionViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            var repositoryResponse = await _prescriptionsService.UpdateAsync(id, PrescriptionModelMapper.MapFromViewModel(prescriptionViewModel));
            if (repositoryResponse.RepositoryResponse == RepositoryResponse.NotFound) return NotFound();
            if (repositoryResponse.RepositoryResponse == RepositoryResponse.Error) return StatusCode(StatusCodes.Status500InternalServerError, repositoryResponse.Message);

            return StatusCode(StatusCodes.Status200OK);
        }
        /// <summary>
        /// Deletes a prescription
        /// </summary>
        /// <remarks>
        ///  DELETE: api/devices/{serialNumber}/prescriptions/{id}
        /// </remarks>
        /// <returns> status code with optional error message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PrescriptionViewModel>> DeletePrescription([FromRoute] string serialNumber, [FromRoute] int id)
        {

            var repositoryResponse = await _prescriptionsService.DeleteAsync(id);
            if (repositoryResponse.RepositoryResponse == RepositoryResponse.NotFound) return NotFound();
            if (repositoryResponse.RepositoryResponse == RepositoryResponse.Error) return StatusCode(StatusCodes.Status500InternalServerError, repositoryResponse.Message);

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
                Flag = Convert.ToInt32(c.Flag)
            }));
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
                DeviceSerialNumber = serialNumber,
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes.Select(ts => TimeSpan.Parse(ts)).ToList()
            });
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
