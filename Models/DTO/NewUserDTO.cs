using System;
using System.Collections.Generic;

namespace Organization_Service.Models.DTO
{
    public class NewUserDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? OfficeID { get; set; }

        public List<int> RolesID { get; set; }
    }
}
