using System;
using System.Collections.Generic;

namespace Organization_Service.Models
{

    /**
     * This class is a DTO of user excluding the password, and Salt. It is used
     * as user response to avoid returning credentials information.
     */
    public class UserResponseDTO
    {

        public int ID { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? OfficeID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<RoleEntity> Roles { get; set; }
    }
}
