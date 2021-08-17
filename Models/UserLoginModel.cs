using System;
namespace Organization_Service.Models
{

    /**
     * This class is the credential user should use to login.
     */
    public class UserLoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
