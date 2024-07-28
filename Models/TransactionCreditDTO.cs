namespace PrestamosCreciendo.Models
{
    public class TransactionCreditDTO
    {
        public int credit_id { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string province { get; set; }
        public DateTime created_at { get; set; }
        public float utility { get; set; }
        public int payment_number { get; set; }
        public float amount_neto { get; set; }
    }
}
