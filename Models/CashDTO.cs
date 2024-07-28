namespace PrestamosCreciendo.Models
{
    public class CashDTO
    {
        public List<SupervisorHasAgentDTO> clients { get; set; }
        public List<CloseDay> report { get; set; }
        public float sum { get; set; }
    }
}
