using System;
using System.Collections.Generic;

namespace Organization_Service.Models
{
    /*
     * This class is a DTO of user class. It is excluding some information such
     * as the date and the salt.
     * It is used to create, update new user.
     */
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
