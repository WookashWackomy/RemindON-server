using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using RemindONServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RemindONServer.Controllers
{
    [Route("api/devices")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize("ShouldBeAnUser")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DevicesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RemindONDevice>>> Getdevices()
        {
           var currentUser = await _userManager.GetUserAsync(HttpContext.User);
           return await _context.RemindONDevices.Where(device => device.UserId == currentUser.Id).ToListAsync();
        }

        // GET: api/devices/5
        [HttpGet("{serialNumber}")]
        public async Task<ActionResult<RemindONDevice>> GetRemindONDevice(string serialNumber)
        {
            var remindONDevice = await _context.RemindONDevices.FindAsync(serialNumber);

            if (remindONDevice == null)
            {
                return NotFound();
            }

            return remindONDevice;
        }

        // PUT: api/devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkserialNumber=2123754
        [HttpPut("{serialNumber}")]
        public async Task<IActionResult> PutRemindONDevice(string serialNumber, RemindONDevice remindONDevice)
        {
            if (serialNumber != remindONDevice.SerialNumber)
            {
                return BadRequest();
            }

            _context.Entry(remindONDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemindONDeviceExists(serialNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/devices
        [HttpPost]
        public async Task<ActionResult<RemindONDevice>> PostRemindONDevice(RemindONDevice remindONDevice)
        {
            _context.RemindONDevices.Add(remindONDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRemindONDevice", new { serialNumber = remindONDevice.SerialNumber }, remindONDevice);
        }

        // DELETE: api/devices/5
        [HttpDelete("{serialNumber}")]
        public async Task<IActionResult> DeleteRemindONDevice(string serialNumber)
        {
            var remindONDevice = await _context.RemindONDevices.FindAsync(serialNumber);
            if (remindONDevice == null)
            {
                return NotFound();
            }

            _context.RemindONDevices.Remove(remindONDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RemindONDeviceExists(string serialNumber)
        {
            return _context.RemindONDevices.Any(e => e.SerialNumber == serialNumber);
        }
    }
}
