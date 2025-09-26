
namespace StilSepetiApp.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string CardNumber { get; set; }
        public string CardPassword { get; set; }
    }
}