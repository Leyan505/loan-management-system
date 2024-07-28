namespace PrestamosCreciendo.Models
{
    public class ClientEditDTO
    {
        public List<Countries>? countries {  get; set; }
        public List<WalletDTO>? wallets {  get; set; }
        public List<agentDTO>? agents { get; set; }
        public ErrorViewModel? error { get; set; }
    }
}
