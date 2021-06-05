using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class UserInfoViewModel
    {
        [Required]
        [PersonalData]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        public string SecondName { get; set; }
        [Required]
        [PersonalData]
        public string Email { get; set; }
    }
}
