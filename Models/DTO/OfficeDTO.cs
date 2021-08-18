using System;
namespace Organization_Service.Entities
{
    public class OfficeDTO
    {
        public int ID { get; set; }
        public string OfficeName { get; set; }
        public int? ParentOfficeID { get; set; }
    }
}
