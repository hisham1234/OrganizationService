using System;
namespace Organization_Service.Models.DTO
{
    public class ResponseOfficeDTO
    {
        public int ID { get; set; }
        public string OfficeName { get; set; }
        public int? ParentOfficeID { get; set; }
    }
}
