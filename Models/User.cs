using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Models
{
    [Index(nameof(Email),IsUnique = true)]
    public class User
    {
        public User()
        {
            this.Roles = new HashSet<Role>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passwords must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set;  }

        [Required]
        [StringLength(50, ErrorMessage = "FirstName cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "LastName cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        public int? OfficeID { get; set; }
        public Office Office { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        public ICollection<Role> Roles { get; set; }
    }

    public class UserDTO
    {
        public int ID { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? OfficeID { get; set; }

        public List<int> RolesID { get; set; }
    }
}
