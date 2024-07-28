namespace PrestamosCreciendo.Models
{
    public class TransactionSummaryDTO
    {
        public string name {  get; set; }
        public string last_name { get; set; }
        public int payment_number { get; set; }
        public float utility { get; set; }
        public float amount_neto { get; set; }
        public int id_credit { get; set; }
        public int number_index { get; set; }
        public float amount { get; set; }
        public float total_payment { get; set; }
        public DateTime created_at { get; set; }
    }
}
