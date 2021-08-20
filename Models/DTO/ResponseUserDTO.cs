using System;
using System.Collections.Generic;

namespace Organization_Service.Models.DTO
{
    /*
     * This class is a DTO of user class. It is excluding some information such
     * as the date and the salt.
     * It is used to create, update new user.
     */
    public class ResponseUserDTO
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ResponseOfficeDTO Office { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public uint RefreshRate { get; set; }
        public ICollection<ResponseRoleDTO> Roles { get; set; }

    }
}
