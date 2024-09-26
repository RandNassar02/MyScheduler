﻿using System.ComponentModel.DataAnnotations;

namespace MyGP2webapp.Models
{
    public class LoginStuden
    {

        [Required(ErrorMessage = "please fill data")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "please fill data")]
        public string password { get; set; }
    }
}
