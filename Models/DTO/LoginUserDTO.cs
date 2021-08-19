using System;
namespace Organization_Service.Models.DTO
{

    /**
     * This class is the credential user should use to login.
     */
    public class LoginUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
