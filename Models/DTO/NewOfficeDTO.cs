using System;
using System.Collections.Generic;

namespace Organization_Service.Models.DTO
{
    public class NewOfficeDTO
    {
        public string OfficeName { get; set; }
        public int? ParentOfficeID { get; set; }
    }
}
