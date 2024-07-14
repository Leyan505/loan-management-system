namespace PrestamosCreciendo.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow.Date;
        public int Country {  get; set; }
        public string City { get; set; }
    }
}
