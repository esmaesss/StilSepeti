using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class ReturnRequestdto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Reason { get; set; }
        public ReturnStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
