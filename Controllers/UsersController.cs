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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemindONServer.Auth;

namespace RemindONServer.Controllers
{
    [Route("api/user")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize("ShouldBeAnUser")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: api/user/info
        [HttpGet("info")]
        public async Task<ActionResult<UserInfoViewModel>> GetUser()
        {
            var identityUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return new UserInfoViewModel { FirstName = identityUser.FirstName, SecondName = identityUser.LastName, Email = identityUser.Email };
        }


        // PUT: api/user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("info")]
        public async Task<IActionResult> PutUser([FromBody] RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            var user = _context.User.Where(u => u.Email == userModel.Email).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.Email = userModel.Email != null ? userModel.Email : user.Email;
                user.FirstName = userModel.FirstName != null ? userModel.FirstName : user.FirstName;
                user.LastName = userModel.SecondName != null ? userModel.SecondName : user.LastName;
                user.PasswordHash = userModel.Password != null ? _userManager.PasswordHasher.HashPassword(user, userModel.Password) : user.PasswordHash;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userModel.Email))
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

        // POST: api/user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUser([FromBody] RegisterViewModel registerModel)
        {
            //_context.User.Add(user);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetUser", new { id = user.Id }, user);

            if (!ModelState.IsValid || registerModel == null)
            {
                return new BadRequestObjectResult(new { Message = "User Registration Failed" });
            }

            var identityUser = new User { UserName = registerModel.Email, FirstName = registerModel.FirstName, LastName = registerModel.SecondName, Email = registerModel.Email };
            var result = await _userManager.CreateAsync(identityUser, registerModel.Password);
            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }

                return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = dictionary });
            }

            return Ok(new { Message = "User Reigstration Successful" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginViewModel loginModel)
        {
            if (!ModelState.IsValid || loginModel == null)
            {
                return new BadRequestObjectResult(new { Message = "Login failed" });
            }

            var identityUser = await _userManager.FindByEmailAsync(loginModel.Email);
            if (identityUser == null)
            {
                return new BadRequestObjectResult(new { Message = "Login failed - user has not been found" });
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, loginModel.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return new BadRequestObjectResult(new { Message = "Login failed" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, identityUser.Email),
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id.ToString()),
                new Claim (ClaimTypes.Role, Roles.StandardUser)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Ok(new { Message = "You are logged in" });
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "You are logged out" });
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string email)
        {
            return _context.User.Any(e => e.Email == email);
        }
    }
}
