using System;
namespace Organization_Service.Models
{
    public class OfficeDTO
    {
        public int ID { get; set; }
        public string OfficeName { get; set; }
        public int? ParentOfficeID { get; set; }
    }
}
