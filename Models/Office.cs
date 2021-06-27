using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization_Service.Models
{
    public class Office
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "OfficeName cannot be longer than 50 characters.")]
        public string OfficeName { get; set; }

        [Column(TypeName="datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; }
    }

    public class OfficeDTO
    {
        public int ID { get; set; }
        public string OfficeName { get; set; }
    }
}
