namespace PrestamosCreciendo.Models
{
    public class Bills
    {
        public int Id { get; set; }
        public DateTime Created_at { get; set; }
        public string Description { get; set; }
        public int Id_agent { get; set; }
        public float Amount { get; set; }
        public int Type { get; set; }
        public int Id_wallet { get; set; }
    }
}
