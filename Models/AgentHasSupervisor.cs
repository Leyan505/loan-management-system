namespace PrestamosCreciendo.Models
{
    public class AgentHasSupervisor
    {
        public int Id { get; set; }
        public int IdAgent {  get; set; }
        public int IdSupervisor { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow.Date;
        public float Base {  get; set; }
        public int IdWallet { get; set; }

    }
}
