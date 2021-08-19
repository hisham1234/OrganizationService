using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Entities
{
    [Table("Office")]
    [Index(nameof(OfficeName), IsUnique = true)]
    public class OfficeEntity
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "OfficeName cannot be longer than 50 characters.")]
        public string OfficeName { get; set; }

        public int? ParentOfficeID { get; set; }
        public OfficeEntity ParentOffice { get; set; }

        [Column(TypeName="datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserEntity> Users { get; set; }
    }
}
