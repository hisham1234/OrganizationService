//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;
//using Organization_Service.Models;

//namespace Organization_Service.Models
//{
//    public class UserModel
//    {
//        public int ID { get; set; }
//        public string Email { get; set; }
//        public string Password { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public int? OfficeID { get; set; }
//        public OfficeModel Office { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime UpdatedAt { get; set; }

//        public ICollection<RoleModel> Roles { get; set; }
//    }
//}
