using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    public class User
    {
        public long UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int UserRole { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        public string GetRole()
        {
            if (UserRole >= 50)
                return "Administrator";
            if (UserRole == 49)
                return "Instructor";
            return "";
        }

        [NotMapped]
        public string Designation { get; set; }
    }
}
