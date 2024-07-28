namespace PrestamosCreciendo.Models
{
    public class SupervisorBillsCreateDTO
    {
        public List<Wallet>? wallets { get; set; }
        public List<ListBill>? bills { get; set; }
        public List<Users> agents { get; set; }
        public ErrorViewModel? error { get; set; }
    }
}
