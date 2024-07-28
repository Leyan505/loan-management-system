namespace PrestamosCreciendo.Models
{
    public class TransactionDTO
    {
        public List<TransactionSummaryDTO> summary { get; set; }
        public List<TransactionCreditDTO> credit { get; set; }
        public List<Bills> bills { get; set; }
        public float total_summary { get; set; }
        public float total_bills { get; set; }
        public float total_credit { get; set; }
    }
}
