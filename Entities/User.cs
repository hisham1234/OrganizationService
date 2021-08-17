using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Models
{
    [Index(nameof(Email),IsUnique = true)]
    public class UserEntity
    {
        public UserEntity()
        {
            this.Roles = new HashSet<RoleEntity>();
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
        public OfficeEntity Office { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [MaxLength(16   )]
        public Byte[] Salt { get; set; }

        public ICollection<RoleEntity> Roles { get; set; }
    }
}
