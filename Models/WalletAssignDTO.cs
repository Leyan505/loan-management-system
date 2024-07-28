namespace PrestamosCreciendo.Models
{
    public class WalletAssignDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
        public int Country { get; set; }
        public string City { get; set; }
        public bool ocuped { get; set; }
    }
}
