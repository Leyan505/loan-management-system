namespace PrestamosCreciendo.Models
{
    public class SupervisorHasAgentDTO
    {
        public Users? user { get; set; }
        public string? wallet_name { get; set; }
        public float base_total {  get; set; }
        public DateTime? wallet_createdAt {  get; set; }
        public Wallet? wallet { get; set; }
    }
}
