using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Models
{
    [Index(nameof(RoleName), IsUnique = true)]
    public class Role
    {
        public Role()
        {
            this.Users = new HashSet<User>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "OfficeName cannot be longer than 50 characters.")]
        public string RoleName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; }
    }

    public class RoleDTO
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
    }
}
