using GroupProject.Constants;
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

        [MaxLength(255)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int _Gender { get; set; }

        public string Gender
        {
            get
            {
                if (_Gender == 1)
                    return "Male";
                else if (_Gender == 2)
                    return "Female";
                return "Others";
            }
        }

        [Required]
        public string AddressLine1 { get; set; }

        [Required]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(255)]
        public string City { get; set; }

        [Required]
        [MaxLength(255)]
        public string State { get; set; }

        [Required]
        public int Pincode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Country { get; set; }

        [MaxLength(10)]
        public int? WorkPhone { get; set; }

        [MaxLength(10)]
        public int? Phone { get; set; }

        [Required]
        public int _UserRole { get; set; }

        [Required]
        public bool ActivatedFlag { get; set; }

        public bool DeletedFlag { get; set; }

        public string UserRole
        {
            get
            {
                if (_UserRole == UserRoles.Administrator)
                    return "Administrator";
                if (_UserRole == UserRoles.Instructor)
                    return "Instructor";
                return "";
            }
        }

        public double? Height { get; set; }

        public double? Weight { get; set; }

        [NotMapped]
        public string Designation { get; set; }
    }
}
