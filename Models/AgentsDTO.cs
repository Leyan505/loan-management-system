namespace PrestamosCreciendo.Models
{
    public class AgentsDTO
    {
        public List<UsersAssignDTO>? AgentsList;
        public List<WalletAssignDTO>? WalletsList;
        public int? SupervisorId;
        public ErrorViewModel? Error { get; set; }
    }
}
