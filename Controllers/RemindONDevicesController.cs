using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<RemindONDeviceViewModel>>> Getdevices()
        {
           var currentUser = await _userManager.GetUserAsync(HttpContext.User);
           return await _context.RemindONDevices.Where(device => device.UserId == currentUser.Id).Select(d => new RemindONDeviceViewModel
           {
               SerialNumber = d.SerialNumber,
               UserId = d.UserId,
               Description = d.Description
           }).ToListAsync();
        }

        // GET: api/devices/5
        [HttpGet("{serialNumber}")]
        public async Task<ActionResult<RemindONDeviceViewModel>> GetRemindONDevice(string serialNumber)
        {
            var remindONDevice = await _context.RemindONDevices.FindAsync(serialNumber);

            if (remindONDevice == null)
            {
                return NotFound();
            }

            return new RemindONDeviceViewModel
            {
                SerialNumber = remindONDevice.SerialNumber,
                UserId = remindONDevice.UserId,
                Description = remindONDevice.Description
            };
        }

        // PUT: api/devices/5
        [HttpPut("{serialNumber}")]
        public async Task<IActionResult> PutRemindONDevice(string serialNumber, RemindONDeviceViewModel remindONDeviceViewModel)
        {
            if (serialNumber != remindONDeviceViewModel.SerialNumber)
            {
                return BadRequest();
            }
            var dbDevice = await _context.RemindONDevices.FindAsync(serialNumber);

            _context.Entry(remindONDeviceViewModel).State = EntityState.Modified;

            try
            {
                dbDevice.SerialNumber = remindONDeviceViewModel.SerialNumber;
                dbDevice.UserId = remindONDeviceViewModel.UserId;
                dbDevice.Description = remindONDeviceViewModel.Description;
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
        public async Task<ActionResult<RemindONDevice>> PostRemindONDevice(RemindONDeviceViewModel remindONDevice)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);



            var dbDevice = await _context.RemindONDevices.FindAsync(remindONDevice.SerialNumber);
            if (dbDevice == null)
                return NotFound();
            dbDevice.UserId = currentUser.Id;
            dbDevice.Description = remindONDevice.Description;

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
            remindONDevice.UserId = null;
            remindONDevice.Description = null;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RemindONDeviceExists(string serialNumber)
        {
            return _context.RemindONDevices.Any(e => e.SerialNumber == serialNumber);
        }
    }
}
