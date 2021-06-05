using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class RegisterViewModel
    {
        [Required]
        [PersonalData]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Last name")]
        public string SecondName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
